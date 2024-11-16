using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanningPoker.Api.Cache;
using PlanningPoker.Api.Exceptions;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.Repositories;
using PlanningPoker.Auth.Models;
using Microsoft.Extensions.Logging;

namespace PlanningPoker.Api.Services;

public class EstimationService : IEstimationService
{
    private readonly IEstimationRepository _estimationRepository;
    private readonly List<Player> _players = new();
    private readonly object _lock = new();
    private readonly ILogger<EstimationService> _logger;

    public EstimationService(IEstimationRepository estimationRepository, ILogger<EstimationService> logger)
    {
        _estimationRepository = estimationRepository;
        _logger = logger;
    }

    public void JoinGame(string gameId, User user)
    {
        var game = _estimationRepository.GetGameById(gameId);

        if (game.Players.ContainsKey(user.Id))
        {
            // already joined
            return;
        }

        var player = new Player();

        if (!game.Players.TryAdd(player.Id, player))
        {
            throw new Exception("Failed to join game");
        }

    }

    public Game StartNewGame(Game game)
    {
       return _estimationRepository.StartNewGame(game);
    }

    public Task<List<Player>> GetPlayers()
    {
        lock (_lock)
        {
            _logger.LogInformation("Getting players. Current count: {Count}", _players.Count);
            return Task.FromResult(_players.ToList());
        }
    }

    public Task<Player?> GetPlayerByConnectionId(string connectionId)
    {
        lock (_lock)
        {
            var player = _players.FirstOrDefault(p => p.ConnectionId == connectionId);
            _logger.LogInformation("Getting player by connection ID: {ConnectionId}. Found: {Found}", 
                connectionId, player != null);
            return Task.FromResult(player);
        }
    }

    public Task AddPlayer(Player player)
    {
        lock (_lock)
        {
            _logger.LogInformation("Adding player: {PlayerName} (ConnectionId: {ConnectionId})", 
                player.Name, player.ConnectionId);
            
            // Remove any existing player with the same name or connection ID
            var existingByName = _players.FirstOrDefault(p => p.Name == player.Name);
            var existingByConnection = _players.FirstOrDefault(p => p.ConnectionId == player.ConnectionId);
            
            if (existingByName != null)
            {
                _logger.LogInformation("Removing existing player with same name: {PlayerName}", player.Name);
                _players.Remove(existingByName);
            }
            
            if (existingByConnection != null && existingByConnection != existingByName)
            {
                _logger.LogInformation("Removing existing player with same connection ID: {ConnectionId}", 
                    player.ConnectionId);
                _players.Remove(existingByConnection);
            }
            
            _players.Add(player);
            _logger.LogInformation("Player added. New count: {Count}", _players.Count);
            return Task.CompletedTask;
        }
    }

    public Task UpdatePlayer(Player player)
    {
        lock (_lock)
        {
            var existingPlayer = _players.FirstOrDefault(p => p.ConnectionId == player.ConnectionId);
            if (existingPlayer != null)
            {
                _logger.LogInformation("Updating player {PlayerName} card to: {Card}", 
                    player.Name, player.Card);
                existingPlayer.Card = player.Card;
            }
            else
            {
                _logger.LogWarning("Player not found for update: {ConnectionId}", player.ConnectionId);
            }
            return Task.CompletedTask;
        }
    }

    public Task RemovePlayerByConnectionId(string connectionId)
    {
        lock (_lock)
        {
            var player = _players.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (player != null)
            {
                _logger.LogInformation("Removing player: {PlayerName} (ConnectionId: {ConnectionId})", 
                    player.Name, connectionId);
                _players.Remove(player);
            }
            else
            {
                _logger.LogWarning("Player not found for removal: {ConnectionId}", connectionId);
            }
            return Task.CompletedTask;
        }
    }

    public Task ResetCards()
    {
        lock (_lock)
        {
            _logger.LogInformation("Resetting cards for all players");
            foreach (var player in _players)
            {
                player.Card = null;
            }
            return Task.CompletedTask;
        }
    }
}
