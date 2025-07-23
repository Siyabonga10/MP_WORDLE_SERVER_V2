using Microsoft.AspNetCore.Authorization;
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
        public IActionResult CreateGame()
        {
            return NotFound("Not yet implemented");
        }

        [HttpPut]
        public IActionResult AddPlayer()
        {
            return NotFound("Not yer implemented");
        }
    }
}