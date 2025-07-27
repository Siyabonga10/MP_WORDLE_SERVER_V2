
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] Player player)
        {
            var result = await _playerService.CreatePlayer(player.Username, player.Password);
            if (result.NewPlayer == null)
                return Conflict(result.OutcomeMsg);
            _playerService.AddJWTToPlayer(player, Response);

            return Ok(result.OutcomeMsg);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Player player)
        {
            Player? authenticated_player = await _playerService.GetPlayerFromCredentials(player.Username, player.Password);
            if (authenticated_player == null)
                return Unauthorized("Invalid credentials");

            _playerService.AddJWTToPlayer(player, Response);
            return Ok("Login succesful");
        }

        [HttpGet]
        [Authorize]
        public IActionResult TestEndpoint()
        {
            return Ok("Authorized");
        }
    }
}