using System;
using PlanningPoker.Api.Cache;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Repositories;

public class EstimationRepository : IEstimationRepository
{
    private readonly IActiveGames _activeGames;

    public EstimationRepository(IActiveGames activeGames)
    {
        _activeGames = activeGames;
    }

    public Game GetGameById(string gameId)
    {
        return _activeGames.GetGame(gameId);
    }

    public Game StartNewGame(Game game)
    {
        if (_activeGames.GameExists(game.Id))
            throw new Exception("Game already exists");
        
        if (!_activeGames.TryAddGame(game.Id, game))
            throw new Exception("Failed to create game");

        return game;
    }
}
