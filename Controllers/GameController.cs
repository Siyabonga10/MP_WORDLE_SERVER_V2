using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using MP_WORDLE_SERVER_V2.Constants;
using Microsoft.AspNetCore.Mvc;
using MP_WORDLE_SERVER_V2.Services;

namespace MP_WORDLE_SERVER_V2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GameController(GameManagementService GameService, IWordManager wordManager) : ControllerBase
    {
        private readonly GameManagementService _GameService = GameService;
        private readonly IWordManager _WordManager = wordManager;

        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var playerGuid = User.FindFirst("jti")?.Value!;
            var newGame = _GameService.CreateGame();
            var playerAddedAsHost = await _GameService.AddPlayerToGameAsync(newGame.ShortId, playerGuid, username, ishost: true);
            return playerAddedAsHost ? CreatedAtAction(nameof(CreateGame), newGame) : StatusCode(500, "Could not create new game");
        }

        [HttpPut("{gameID}")]
        public async Task<IActionResult> AddPlayer(string gameID)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var playerGuid = User.FindFirst("jti")?.Value!;
            var playerAdded = await _GameService.AddPlayerToGameAsync(gameID, playerGuid, username, ishost: false);
            return playerAdded ? NoContent() : BadRequest("Could not add player to game.");
        }

        [HttpGet("{gameID}")]
        public async Task SubscribeToGameUpdates(string gameID)
        {
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";
            Response.Headers["X-Accel-Buffering"] = "no"; // Disable buffering in proxies
            Response.Headers["Content-Encoding"] = "identity"; // Disable compression

            var playerGuid = User.FindFirst("jti")?.Value!;
            var playerAdded = await _GameService.AddPlayerToGameStreamAsync(gameID, playerGuid, new StreamWriter(Response.Body));
            if (!playerAdded)
                return;

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            await _GameService.SendToAllExcept(gameID, playerGuid, username, EventTypes.PlayerJoined);
            await _GameService.SendPlayersAlreadyInGame(playerGuid, gameID, username, EventTypes.PlayersInGame);
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
            return;
        }

        [HttpPost("{gameID}/start")]
        public async Task<IActionResult> StartGame(string gameID)
        {
            var playerGuid = User.FindFirst("jti")?.Value!;
            if (!_GameService.StartGame(playerGuid, gameID))
                return BadRequest("Could not start game");

            var words = _WordManager.GetRandomWords(5);
            var words_payload = string.Join("\n", words);
            await _GameService.SendToAll(gameID, EventTypes.StartGame, words_payload);

            return Ok("Game started");
        }
    }
}