using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;
using MP_WORDLE_SERVER_V2.Models;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class GameService
    {
        readonly private IDbContextFactory<GameDb> _DbContextFactory;
        readonly private IDbContextFactory<GameCache> _DbContextFactoryLive;
        public GameService(IDbContextFactory<GameDb> dBCtxFactory, IDbContextFactory<GameCache> dbContextFactoryLive)
        {
            _DbContextFactory = dBCtxFactory;
           _DbContextFactoryLive = dbContextFactoryLive;
        }

        public async Task<Game> CreateGameAsync()
        {
            using var dbCtx = _DbContextFactoryLive.CreateDbContext();
            var newGame = new Game(Guid.NewGuid());
            await dbCtx.Games.AddAsync(newGame);
            await dbCtx.SaveChangesAsync();
            return newGame;
        }

        public async Task<bool> AddPlayerToGameAsync(string gameId, string playerId, bool ishost)
        {
            Guid gameGUID = new(gameId);
            Guid playerGUID = new(playerId);

            using var dbCtx = _DbContextFactoryLive.CreateDbContext();
            using var playerDb = _DbContextFactory.CreateDbContext();
            var targetGame = await dbCtx.Games.FirstOrDefaultAsync(game => game.Id == gameGUID);
            var targetPlayer = await playerDb.Players.FirstOrDefaultAsync(player => player.Id == playerGUID);

            if (targetGame == null || targetPlayer == null)
                return false;

            if (targetGame.State == GameState.WAITING_FOR_PLAYERS)
            {
                var playerAdded = targetGame.AddPlayer(playerGUID);
                if (!playerAdded) return false;
                var gamePlayerJunction = new GamePlayerJunction()
                {
                    GameId = gameGUID,
                    PlayerId = playerGUID,
                    Game = targetGame,
                    Player = targetPlayer
                };
                await dbCtx.GamePlayerJunctions.AddAsync(gamePlayerJunction);
            }
            else
                return false;

            if (ishost)
            {
                targetGame.HostId = targetPlayer.Id;
                dbCtx.Games.Update(targetGame);
            }

            await dbCtx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateGameStateAsync(string gameId, GameState newState)
        {
            Guid gameGUID = new(gameId);

            using var dbCtx = _DbContextFactoryLive.CreateDbContext();

            var targetGame = await dbCtx.Games.FirstOrDefaultAsync(game => game.Id == gameGUID);

            if (targetGame == null || targetGame.State >= newState) // State progresses forward, assumes states are ordered which is currently the case
                return false;

            targetGame.State = newState;
            dbCtx.Games.Update(targetGame);
            await dbCtx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddPlayerToGameStreamAsync(string gameGUID, string playerGUID, StreamWriter playerWriter)
        {
            Guid GameGUID = new(gameGUID);
            Guid PlayerGUID = new(playerGUID);

            using var dbCtx = _DbContextFactoryLive.CreateDbContext();
            using var playerDb = _DbContextFactory.CreateDbContext();

            var targetGame = await dbCtx.Games.FirstOrDefaultAsync(game => game.Id == GameGUID);
            var targetPlayer = await playerDb.Players.FirstOrDefaultAsync(player => player.Id == PlayerGUID);

            if (targetGame == null || targetPlayer == null)
                return false;

            if (!targetGame.GetAllPlayers().Any(player => player == PlayerGUID))
                return false;

            if (targetGame.PlayerConnections.Any(conn => conn.Value == playerWriter))
                return false;

            targetGame.PlayerConnections.Add(playerGUID, playerWriter);
            dbCtx.Games.Update(targetGame);
            return true;
        }
    }
}
