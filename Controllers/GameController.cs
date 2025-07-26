using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MP_WORDLE_SERVER_V2.Services;

namespace MP_WORDLE_SERVER_V2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GameController(GameManagementService GameService) : ControllerBase
    {
        private readonly GameManagementService _GameService = GameService;

        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var playerGuid = User.FindFirst("jti")?.Value!;
            var newGame = _GameService.CreateGame();
            var playerAddedAsHost = await _GameService.AddPlayerToGameAsync(newGame.Id.ToString(), playerGuid, username, ishost: true);
            return playerAddedAsHost ? CreatedAtAction(nameof(CreateGame), newGame) : StatusCode(500, "Could not create new game");
        }

        [HttpPut("{gameID}")]
        public async Task<IActionResult> AddPlayer(string gameID)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var playerGuid = User.FindFirst("jti")?.Value!;
            var playerAdded = await _GameService.AddPlayerToGameAsync(gameID, playerGuid, username,  ishost: false);
            return playerAdded ? NoContent() : BadRequest("Could not add player to game");
        }

        [HttpGet("{gameID}")]
        public async Task<IActionResult> SubscribeToGameUpdates(string gameID)
        {
            var playerGuid = User.FindFirst("jti")?.Value!;
            var playerAdded = await _GameService.AddPlayerToGameStreamAsync(gameID, playerGuid, new StreamWriter(Response.Body));
            if (!playerAdded)
                return Unauthorized("Could not subscribe to game events");

            Response.Headers.ContentType = "text/event-stream";
            Response.Headers.CacheControl = "no-cache";
            Response.Headers.Connection = "keep-alive";

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            await _GameService.SendToAllExcept(gameID, playerGuid, username);
            await _GameService.SendPlayersAlreadyInGame(playerGuid, gameID, username);
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
            return new JsonResult("Game stream closed");
        }
    }
}