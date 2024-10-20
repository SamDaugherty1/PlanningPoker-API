using System;
using System.Collections.Concurrent;
using PlanningPoker.Api.Exceptions;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Cache;

public static class ActiveGames
{
    public static readonly ConcurrentDictionary<string, Game> Games = new();

    public static Game GetGame(string gameId)
    {
        if (!Games.TryGetValue(gameId, out var game))
        {
            throw new NoActiveGameException();
        }

        return game;
    }
}
