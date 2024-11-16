using System;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Repositories;

public interface IEstimationRepository
{
    public Game GetGameById(string gameId);
    public Game StartNewGame(Game game);
}
