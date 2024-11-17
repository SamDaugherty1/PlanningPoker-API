using System.Collections.Generic;
using System.Threading.Tasks;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Services;

public interface IEstimationService
{
    Task<Game> JoinGame(string gameId, Player player);
    Task<Player?> GetPlayerByConnectionId(string connectionId);
    Task RemovePlayerByConnectionId(string connectionId);
    Task<List<Player>> GetGamePlayers(string gameId);
    Task SetPlayerCard(string playerId, int? card);
    Task ShowCards(string gameId);
    Task ResetCards(string gameId);
}
