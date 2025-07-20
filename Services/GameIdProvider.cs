using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class GameIdProvider
    {
        readonly private GameDb _DbContext;
        private int AvailableId = 0;
        public GameIdProvider(GameDb dbContext)
        {
            _DbContext = dbContext;
        }

        public async Task InitProvider()
        {
            var lastGame = await _DbContext.Games
                .OrderByDescending(g => g.Id)
                .FirstAsync();
            if (lastGame == null) return;
            AvailableId = lastGame.Id;
        }

        public int GetNewId()
        {
            return AvailableId++;
        }
    }
}