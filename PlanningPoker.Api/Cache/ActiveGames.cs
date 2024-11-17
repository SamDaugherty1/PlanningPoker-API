using System;
using System.Collections.Concurrent;
using PlanningPoker.Api.Exceptions;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Cache;

public interface IActiveGames
{
    Game GetGame(string gameId);
    bool TryAddGame(string gameId, Game game);
    bool TryGetGame(string gameId, out Game game);
    bool GameExists(string gameId);
}

public class ActiveGames : IActiveGames
{
    private readonly ConcurrentDictionary<string, Game> _games = new();

    public Game GetGame(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var game))
        {
            throw new NoActiveGameException();
        }

        return game;
    }

    public bool TryAddGame(string gameId, Game game)
    {
        return _games.TryAdd(gameId, game);
    }

    public bool TryGetGame(string gameId, out Game game)
    {
        return _games.TryGetValue(gameId, out game);
    }

    public bool GameExists(string gameId)
    {
        return _games.ContainsKey(gameId);
    }
}
