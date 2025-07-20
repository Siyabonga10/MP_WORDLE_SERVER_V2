using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class PlayerService
    {
        readonly private GameDb _DbContext;
        private int AvailableId = 0;
        public PlayerService(GameDb dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task InitPlayerService()
        {
            var lastPlayer = await _DbContext.Players
                .OrderByDescending(player => player.Id)
                .FirstAsync();
            if (lastPlayer == null) return;
            AvailableId = lastPlayer.Id;
        }

        private int GetNewId()
        {
            return AvailableId++;
        }
    }
}