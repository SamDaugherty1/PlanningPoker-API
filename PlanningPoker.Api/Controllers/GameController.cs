using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.Services;

namespace PlanningPoker.Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IEstimationService _estimationService;
        private readonly ILogger _logger;
        public GameController(ILogger logger, IEstimationService estimationService)
        {
            _logger = logger;
            _estimationService = estimationService;
        }
        [HttpPost]
        [Route("new")]
        public ActionResult StartNewGame([FromBody] Game game) 
        {
            try
            {
                return Ok(_estimationService.StartNewGame(game));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unkown error");
                return BadRequest();
            }
        }   
    }
}
