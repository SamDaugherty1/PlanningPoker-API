using System;
using PlanningPoker.Api.Cache;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Repositories;

public class EstimationRepository : IEstimationRepository
{
    public Game GetGameById(string gameId)
    {
        return ActiveGames.GetGame(gameId);
    }

    public Game StartNewGame(Game game)
    {
        if (ActiveGames.Games.ContainsKey(game.Id))
            throw new Exception("Game already exists");
        
        ActiveGames.Games.TryAdd(game.Id, game);

        return game;
    }
}
