using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Auth.Models;
using PlanningPoker.Auth.Services;

namespace PlanningPoker.Api.Controllers
{
    [Route("Api")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public Authentication(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody] User  user)
        {
            return Ok(_tokenService.GenerateToken(user));
        }
    }
}
