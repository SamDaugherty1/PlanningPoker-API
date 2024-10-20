using System;
using PlanningPoker.Auth.Models;

namespace PlanningPoker.Api.Services;

public interface IEstimationService
{
    public void JoinGame(string gameId, User user);
}
