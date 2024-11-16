using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanningPoker.Api.Cache;
using PlanningPoker.Api.Exceptions;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace PlanningPoker.Api.Services;

public class EstimationService : IEstimationService
{
    private readonly IEstimationRepository _estimationRepository;
    private readonly Dictionary<string, Game> _games = new();
    private readonly Dictionary<string, Player> _players = new();
    private readonly object _lock = new();
    private readonly ILogger<EstimationService> _logger;

    public EstimationService(IEstimationRepository estimationRepository, ILogger<EstimationService> logger)
    {
        _estimationRepository = estimationRepository;
        _logger = logger;
    }

    public Task<Game> JoinGame(string gameId, Player player)
    {
        lock (_lock)
        {
            // Create game if it doesn't exist
            if (!_games.ContainsKey(gameId))
            {
                _games[gameId] = new Game
                {
                    Id = gameId,
                    Players = new Dictionary<string, Player>()
                };
            }

            var game = _games[gameId];

            // Remove player from any other games
            foreach (var g in _games.Values)
            {
                g.Players.Remove(player.Id);
            }

            // Add player to game
            game.Players[player.Id] = player;
            _players[player.ConnectionId] = player;

            return Task.FromResult(game);
        }
    }

    public Task<Player?> GetPlayerByConnectionId(string connectionId)
    {
        lock (_lock)
        {
            return Task.FromResult(_players.GetValueOrDefault(connectionId));
        }
    }

    public Task RemovePlayerByConnectionId(string connectionId)
    {
        lock (_lock)
        {
            if (_players.TryGetValue(connectionId, out var player))
            {
                if (_games.TryGetValue(player.GameId, out var game))
                {
                    game.Players.Remove(player.Id);
                    if (game.Players.Count == 0)
                    {
                        _games.Remove(game.Id);
                    }
                }
                _players.Remove(connectionId);
            }
            return Task.CompletedTask;
        }
    }

    public Task<List<Player>> GetGamePlayers(string gameId)
    {
        lock (_lock)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                return Task.FromResult(new List<Player>(game.Players.Values));
            }
            return Task.FromResult(new List<Player>());
        }
    }

    public Task SetPlayerCard(string playerId, int? card)
    {
        lock (_lock)
        {
            foreach (var game in _games.Values)
            {
                if (game.Players.TryGetValue(playerId, out var player))
                {
                    player.Card = card;
                    break;
                }
            }
            return Task.CompletedTask;
        }
    }

    public Task ShowCards(string gameId)
    {
        lock (_lock)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                game.ShowCards = true;
            }
            return Task.CompletedTask;
        }
    }

    public Task ResetCards(string gameId)
    {
        lock (_lock)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                game.ShowCards = false;
                foreach (var player in game.Players.Values)
                {
                    player.Card = null;
                }
            }
            return Task.CompletedTask;
        }
    }
}
