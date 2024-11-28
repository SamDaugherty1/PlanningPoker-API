using Microsoft.Extensions.Logging;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Services;

public class MockEstimationService : IEstimationService
{
    private readonly Dictionary<string, Game> _games = new();
    private readonly Dictionary<string, Player> _players = new();
    private readonly object _lock = new();
    private readonly ILogger<MockEstimationService> _logger;

    private readonly List<Player> _mockPlayers = new()
    {
        new Player { Id = "1", Name = "Alice Johnson", ConnectionId = "mock-1", Card = 3, ViewOnly = false },
        new Player { Id = "2", Name = "Bob Smith", ConnectionId = "mock-2", Card = 5, ViewOnly = false },
        new Player { Id = "3", Name = "Charlie Brown", ConnectionId = "mock-3", Card = 8, ViewOnly = false },
        new Player { Id = "4", Name = "Diana Prince", ConnectionId = "mock-4", Card = null, ViewOnly = false },
        new Player { Id = "5", Name = "Edward Norton", ConnectionId = "mock-5", Card = 13, ViewOnly = false },
        new Player { Id = "6", Name = "Frank Castle", ConnectionId = "mock-6", Card = 2, ViewOnly = false },
        new Player { Id = "7", Name = "Grace Kelly", ConnectionId = "mock-7", Card = null, ViewOnly = false },
        new Player { Id = "8", Name = "Henry Ford", ConnectionId = "mock-8", Card = 1, ViewOnly = false },
        new Player { Id = "9", Name = "Iris West", ConnectionId = "mock-9", Card = null, ViewOnly = true },
        new Player { Id = "10", Name = "Jack Ryan", ConnectionId = "mock-10", Card = 21, ViewOnly = false },
        new Player { Id = "11", Name = "Kate Bishop", ConnectionId = "mock-11", Card = 34, ViewOnly = false },
        new Player { Id = "12", Name = "Luke Cage", ConnectionId = "mock-12", Card = -1, ViewOnly = false } // -1 represents '?'
    };

    public MockEstimationService(ILogger<MockEstimationService> logger)
    {
        _logger = logger;
    }

    public Task<Game> JoinGame(string gameId, Player player)
    {
        lock (_lock)
        {
            // Create game if it doesn't exist
            if (!_games.ContainsKey(gameId))
            {
                var game = new Game
                {
                    Id = gameId,
                    Players = new Dictionary<string, Player>()
                };

                // Add mock players to the game with the correct gameId
                foreach (var mockPlayer in _mockPlayers)
                {
                    var playerCopy = new Player
                    {
                        Id = mockPlayer.Id,
                        Name = mockPlayer.Name,
                        ConnectionId = mockPlayer.ConnectionId,
                        GameId = gameId,
                        Card = mockPlayer.Card,
                        ViewOnly = mockPlayer.ViewOnly
                    };
                    game.Players[playerCopy.Id] = playerCopy;
                }

                _games[gameId] = game;
            }

            var existingGame = _games[gameId];

            // Add the real player
            existingGame.Players[player.Id] = player;
            _players[player.ConnectionId] = player;

            _logger.LogInformation("Player {PlayerName} joined game {GameId}", player.Name, gameId);
            return Task.FromResult(existingGame);
        }
    }

    public Task<Player?> GetPlayerByConnectionId(string connectionId)
    {
        lock (_lock)
        {
            return Task.FromResult(_players.GetValueOrDefault(connectionId));
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

    public Task RemovePlayerByConnectionId(string connectionId)
    {
        lock (_lock)
        {
            if (_players.TryGetValue(connectionId, out var player))
            {
                if (_games.TryGetValue(player.GameId, out var game))
                {
                    game.Players.Remove(player.Id);
                    // Don't remove the game even if empty to keep mock players
                }
                _players.Remove(connectionId);
                _logger.LogInformation("Player {PlayerName} left game {GameId}", player.Name, player.GameId);
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
                foreach (var player in game.Players.Values)
                {
                    player.Card = null;
                }
                game.ShowCards = false;
                _logger.LogInformation("Cards reset in game {GameId}", gameId);
            }
            return Task.CompletedTask;
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
                    _logger.LogInformation("Player {PlayerName} selected card {Card}", player.Name, card);
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
                _logger.LogInformation("Cards revealed in game {GameId}", gameId);
            }
            return Task.CompletedTask;
        }
    }
}
