
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
            _playerService.AddJWTToPlayer(player, Response);

            return Ok(result.OutcomeMsg);
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
        public IActionResult TestEndpoint()
        {
            var envs = Environment.GetEnvironmentVariables();
            if (envs == null)
                return Ok("No envs found");

            string env_list = "";
            foreach (System.Collections.DictionaryEntry env in envs)
                env_list += $"{env.Key}\n";
            env_list += $"\n\n {TmpClass.Tmp}";
            return Ok($"envs keys \n {env_list}");
        }
    }
}