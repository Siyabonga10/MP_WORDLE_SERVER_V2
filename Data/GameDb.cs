using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Models;

namespace MP_WORDLE_SERVER_V2.Data
{
    public class GameDb(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GamePlayerJunction> GamePlayerJunctions { get; set; }
    }

    public class GameCache(DbContextOptions options) : GameDb(options);
}