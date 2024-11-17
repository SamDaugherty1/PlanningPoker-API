using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.Services;

namespace PlanningPoker.Api.Hubs;

public class PokerHub : Hub
{
    private readonly IEstimationService _estimationService;
    private readonly ILogger<PokerHub> _logger;

    public PokerHub(IEstimationService estimationService, ILogger<PokerHub> logger)
    {
        _estimationService = estimationService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        var player = await _estimationService.GetPlayerByConnectionId(Context.ConnectionId);
        if (player != null)
        {
            await _estimationService.RemovePlayerByConnectionId(Context.ConnectionId);
            await NotifyPlayersUpdated(player.GameId);
            await Clients.Group(player.GameId).SendAsync("playerLeft", player.Name);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGame(string gameId, string playerName, bool viewOnly = false)
    {
        _logger.LogInformation("Player joining game {GameId}: {PlayerName} (ViewOnly: {ViewOnly})", gameId, playerName, viewOnly);
        
        var player = new Player
        {
            Id = Guid.NewGuid().ToString(),
            Name = playerName,
            ConnectionId = Context.ConnectionId,
            GameId = gameId,
            ViewOnly = viewOnly
        };

        try
        {
            // This will create the game if it doesn't exist
            await _estimationService.JoinGame(gameId, player);
            
            // Add to SignalR group for this game
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            
            _logger.LogInformation("Player added to game {GameId}, notifying game clients", gameId);
            await NotifyPlayersUpdated(gameId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining game {GameId}", gameId);
            throw;
        }
    }

    public async Task SelectCard(int? card)
    {
        var player = await _estimationService.GetPlayerByConnectionId(Context.ConnectionId);
        if (player == null || player.ViewOnly)
        {
            return;
        }

        await _estimationService.SetPlayerCard(player.Id, card);
        await NotifyPlayersUpdated(player.GameId);
        await Clients.Group(player.GameId).SendAsync("cardSelected", player.Name, card);
    }

    public async Task ShowCards()
    {
        var player = await _estimationService.GetPlayerByConnectionId(Context.ConnectionId);
        if (player == null)
        {
            return;
        }

        await _estimationService.ShowCards(player.GameId);
        await Clients.Group(player.GameId).SendAsync("ShowCards");
    }

    public async Task ResetCards()
    {
        var player = await _estimationService.GetPlayerByConnectionId(Context.ConnectionId);
        if (player == null)
        {
            return;
        }

        await _estimationService.ResetCards(player.GameId);
        await Clients.Group(player.GameId).SendAsync("ResetCards");
    }

    private async Task NotifyPlayersUpdated(string gameId)
    {
        var players = await _estimationService.GetGamePlayers(gameId);
        await Clients.Group(gameId).SendAsync("UpdatePlayers", players);
    }
}
