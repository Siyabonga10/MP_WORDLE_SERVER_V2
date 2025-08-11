
using Microsoft.AspNetCore.Authorization;
using MP_WORDLE_SERVER_V2.Models;
using Microsoft.AspNetCore.Mvc;
using MP_WORDLE_SERVER_V2.Services;
using MP_WORDLE_SERVER_V2.Constants;

namespace MP_WORDLE_SERVER_V2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly ILogger<PlayerController> _logger;
        public PlayerController(PlayerService plService, ILogger<PlayerController> logger)
        {
            _playerService = plService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] Player player)
        {
            _logger.LogInformation("endpoint invoked");
            var result = await _playerService.CreatePlayer(player.Username, player.Password);
            if (result.NewPlayer == null)
                return Conflict(result.OutcomeMsg);
            _playerService.AddJWTToPlayer(result.NewPlayer, Response);

            return CreatedAtAction(nameof(CreateAccount), result.NewPlayer);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Player player)
        {
            _logger.LogInformation($"Attempit to log in {player.Username}");
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
            return Ok($"Player authorized");
        }
    }
}