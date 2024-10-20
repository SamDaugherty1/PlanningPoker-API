using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PlanningPoker.Api.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpPost]
        [Route("new")]
        public ActionResult StartNewGame() 
        {
            return Ok();
        }   
    }
}
