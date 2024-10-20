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
}
