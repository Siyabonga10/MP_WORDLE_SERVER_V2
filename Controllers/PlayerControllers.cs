
using Microsoft.AspNetCore.Authorization;
using MP_WORDLE_SERVER_V2.Models;
using Microsoft.AspNetCore.Mvc;
using MP_WORDLE_SERVER_V2.Services;

namespace MP_WORDLE_SERVER_V2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        public PlayerController(PlayerService plService)
        {
            _playerService = plService;
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticatePlayerAsync([FromBody] Player player)
        {
            var token = await _playerService.GenerateJwtTokenAsync(player);

            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,  // Cannot be accessed by JavaScript
                Secure = true,    // HTTPS only
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            return Ok(new { message = "Authentication successful" });
        }

        [HttpGet]
        [Authorize]
        public IActionResult TestEndpoint()
        {
            return Ok("Authorized");
        }
    }
}