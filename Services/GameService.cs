using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class GameService
    {
        readonly private IDbContextFactory<GameDb> _DbContextFactory;
        private int AvailableId = 0;
        public GameService(IDbContextFactory<GameDb> dBCtxFactory)
        {
            _DbContextFactory = dBCtxFactory;
        }

        public async Task InitProvider()
        {
            var dbContext = _DbContextFactory.CreateDbContext();
            var lastGame = await dbContext.Games
                .OrderByDescending(g => g.Id)
                .FirstAsync();
            if (lastGame == null) return;
            AvailableId = lastGame.Id;
        }

        private int GetNewId()
        {
            return AvailableId++;
        }
    }
}