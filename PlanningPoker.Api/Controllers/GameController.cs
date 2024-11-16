using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.Services;

namespace PlanningPoker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IEstimationService _estimationService;
    private readonly ILogger<GameController> _logger;

    public GameController(IEstimationService estimationService, ILogger<GameController> logger)
    {
        _estimationService = estimationService;
        _logger = logger;
    }

    [HttpGet("{gameId}/players")]
    public async Task<ActionResult<List<Player>>> GetPlayers(string gameId)
    {
        try
        {
            var players = await _estimationService.GetGamePlayers(gameId);
            return Ok(players);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players for game {GameId}", gameId);
            return StatusCode(500, "An error occurred while getting players");
        }
    }

    [HttpPost("{gameId}/reset")]
    public async Task<IActionResult> ResetGame(string gameId)
    {
        try
        {
            await _estimationService.ResetCards(gameId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting game {GameId}", gameId);
            return StatusCode(500, "An error occurred while resetting the game");
        }
    }
}
