using System;
using PlanningPoker.Api.Cache;
using PlanningPoker.Api.Exceptions;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.Repositories;
using PlanningPoker.Auth.Models;

namespace PlanningPoker.Api.Services;

public class EstimationService : IEstimationService
{
    private readonly IEstimationRepository _estimationRepository;
    public EstimationService(IEstimationRepository estimationRepository)
    {
        _estimationRepository = estimationRepository;
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
}
