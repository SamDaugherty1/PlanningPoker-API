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
        // Send current players to new client
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        var player = await _estimationService.GetPlayerByConnectionId(Context.ConnectionId);
        if (player != null)
        {
            await _estimationService.RemovePlayerByConnectionId(Context.ConnectionId);
            await NotifyPlayersUpdated();
            await Clients.Others.SendAsync("playerLeft", player.Name);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGame(string playerName, bool viewOnly = false)
    {
        _logger.LogInformation("Player joining: {PlayerName} (ViewOnly: {ViewOnly})", playerName, viewOnly);
        var player = new Player
        {
            Name = playerName,
            ConnectionId = Context.ConnectionId,
            ViewOnly = viewOnly
        };

        await _estimationService.AddPlayer(player);
        _logger.LogInformation("Player added, notifying all clients");
        await NotifyPlayersUpdated();
    }

    public async Task LeaveGame()
    {
        _logger.LogInformation("Player leaving: {ConnectionId}", Context.ConnectionId);
        var player = await _estimationService.GetPlayerByConnectionId(Context.ConnectionId);
        if (player != null)
        {
            await _estimationService.RemovePlayerByConnectionId(Context.ConnectionId);
            await NotifyPlayersUpdated();
            await Clients.Others.SendAsync("playerLeft", player.Name);
        }
    }

    public async Task SelectCard(int? card)
    {
        var player = await _estimationService.GetPlayerByConnectionId(Context.ConnectionId);
        if (player != null)
        {
            _logger.LogInformation("Player {PlayerName} selected card: {Card}", player.Name, card);
            player.Card = card;
            await _estimationService.UpdatePlayer(player);
            await NotifyPlayersUpdated();
            await Clients.All.SendAsync("cardSelected", player.Name, card);
        }
    }

    public async Task ShowCards()
    {
        _logger.LogInformation("Showing cards");
        await Clients.All.SendAsync("updateShowCards", true);
    }

    public async Task ResetCards()
    {
        _logger.LogInformation("Resetting cards");
        await _estimationService.ResetCards();
        await Clients.All.SendAsync("updateShowCards", false);
        await NotifyPlayersUpdated();
    }

    private async Task NotifyPlayersUpdated()
    {
        var players = await _estimationService.GetPlayers();
        _logger.LogInformation("Notifying players updated: {Count} players", players.Count);
        foreach (var player in players)
        {
            _logger.LogInformation("Player in list: {Name} (ConnectionId: {ConnectionId}, Card: {Card})",
                player.Name, player.ConnectionId, player.Card);
        }
        await Clients.All.SendAsync("updatePlayers", players);
    }
}
