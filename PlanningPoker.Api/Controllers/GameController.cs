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

    [HttpGet]
    public async Task<ActionResult<List<Player>>> GetPlayers()
    {
        try
        {
            var players = await _estimationService.GetPlayers();
            return Ok(players);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players");
            return StatusCode(500, "An error occurred while getting players");
        }
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetGame()
    {
        try
        {
            await _estimationService.ResetCards();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting game");
            return StatusCode(500, "An error occurred while resetting the game");
        }
    }
}
