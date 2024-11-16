using System;
using PlanningPoker.Api.Models;
using PlanningPoker.Auth.Models;

namespace PlanningPoker.Api.Services;

public interface IEstimationService
{
    public void JoinGame(string gameId, User user);
    public Game StartNewGame(Game game);
}
