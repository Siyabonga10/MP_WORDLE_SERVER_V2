using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class GameService
    {
        readonly private IDbContextFactory<GameDb> _DbContextFactory;
        public GameService(IDbContextFactory<GameDb> dBCtxFactory)
        {
            _DbContextFactory = dBCtxFactory;
        }
        
    }
}