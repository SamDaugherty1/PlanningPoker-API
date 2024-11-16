using System;
using System.Threading.Tasks;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Services;

public interface IEstimationService
{
    Task<List<Player>> GetPlayers();
    Task<Player?> GetPlayerByConnectionId(string connectionId);
    Task AddPlayer(Player player);
    Task UpdatePlayer(Player player);
    Task RemovePlayerByConnectionId(string connectionId);
    Task ResetCards();
}
