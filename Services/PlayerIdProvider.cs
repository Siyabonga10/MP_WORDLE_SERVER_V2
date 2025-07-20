using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class PlayerIdProvider
    {
        readonly private GameDb _DbContext;
        private int AvailableId = 0;
        public PlayerIdProvider(GameDb dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task InitProvider()
        {
            var lastPlayer = await _DbContext.Players
                .OrderByDescending(player => player.Id)
                .FirstAsync();
            if (lastPlayer == null) return;
            AvailableId = lastPlayer.Id;
        }

        public int GetNewId()
        {
            return AvailableId++;
        }
    }
}