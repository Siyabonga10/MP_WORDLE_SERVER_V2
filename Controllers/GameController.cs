using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MP_WORDLE_SERVER_V2.Services;

namespace MP_WORDLE_SERVER_V2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GameController(GameService GameService) : ControllerBase
    {
        private readonly GameService _GameService = GameService;

        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            var playerGuid = User.FindFirst("jti")?.Value!;
            var newGame = await _GameService.CreateGameAsync();
            var playerAddedAsHost = await _GameService.AddPlayerToGameAsync(newGame.Id.ToString(), playerGuid, ishost: true);
            return playerAddedAsHost ? CreatedAtAction(nameof(CreateGame), newGame) : StatusCode(500, "Could not create new game");
        }

        [HttpPut("{gameID}")]
        public async Task<IActionResult> AddPlayer(string gameID)
        {
            var playerGuid = User.FindFirst("jti")?.Value!;
            var playerAdded = await _GameService.AddPlayerToGameAsync(gameID, playerGuid, ishost: false);
            return playerAdded ? NoContent() : BadRequest("Could not add player to game");
        }

        [HttpGet("{gameID}")]
        public async Task<IActionResult> SubscribeToGameUpdates(string gameID)
        {
            Console.WriteLine($"Attempitng to add {User.FindFirst("sub")?.Value!}");
            var playerGuid = User.FindFirst("jti")?.Value!;
            var playerAdded = await _GameService.AddPlayerToGameStreamAsync(gameID, playerGuid, new StreamWriter(Response.Body));
            if (!playerAdded)
                return Unauthorized("Could not subscribe to game events");
            return new JsonResult("Game stream closed");
        }
    }
}