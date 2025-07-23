using Microsoft.EntityFrameworkCore;
using MP_WORDLE_SERVER_V2.Data;
using MP_WORDLE_SERVER_V2.Models;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class GameService
    {
        readonly private IDbContextFactory<GameDb> _DbContextFactory;
        public GameService(IDbContextFactory<GameDb> dBCtxFactory)
        {
            _DbContextFactory = dBCtxFactory;
        }

        public async Task<Game> CreateGameAsync()
        {
            var dbCtx = _DbContextFactory.CreateDbContext();
            var newGame = new Game(Guid.NewGuid());
            await dbCtx.Games.AddAsync(newGame);
            await dbCtx.SaveChangesAsync();
            return newGame;
        }

        public async Task<bool> AddPlayerToGameAsync(string gameId, string playerId, bool ishost)
        {
            Guid gameGUID = new(gameId);
            Guid playerGUID = new(playerId);

            var dbCtx = _DbContextFactory.CreateDbContext();
            var targetGame = await dbCtx.Games.FirstAsync(game => game.Id == gameGUID);
            var targetPlayer = await dbCtx.Players.FirstAsync(player => player.Id == playerGUID);

            if (targetGame == null || targetPlayer == null)
                return false;

            if (targetGame.State == GameState.WAITING_FOR_PLAYERS)
            {
                targetGame.AddPlayer(targetPlayer);
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

            var dbCtx = _DbContextFactory.CreateDbContext();
            var targetGame = await dbCtx.Games.FirstAsync(game => game.Id == gameGUID);

            if (targetGame == null || targetGame.State >= newState) // State progresses forward, assumes states are ordered which is currently the case
                return false;

            targetGame.State = newState;
            dbCtx.Games.Update(targetGame);
            await dbCtx.SaveChangesAsync();

            return true;
        }
    }
}