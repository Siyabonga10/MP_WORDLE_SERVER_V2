using MP_WORDLE_SERVER_V2.Models;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class GameManagementService
    {
        private readonly List<Game> ActiveGames = [];
        public Game CreateGame()
        {
            var newGame = new Game(Guid.NewGuid());
            ActiveGames.Add(newGame);
            return newGame;
        }

        public async Task<bool> AddPlayerToGameAsync(string gameId, string playerId, bool ishost)
        {
            var res = await Task.Run(() =>
            {
                Guid gameGUID = new(gameId);
                Guid playerGUID = new(playerId);

                var targetGame = ActiveGames.FirstOrDefault(game => game.Id == gameGUID);

                if (targetGame == null)
                    return false;

                if (targetGame.State == GameState.WAITING_FOR_PLAYERS)
                {
                    var playerAdded = targetGame.AddPlayer(playerGUID);
                    if (!playerAdded) return false;
                }
                else
                    return false;

                if (ishost)
                {
                    targetGame.HostId = playerGUID;
                }
                return true;
            });
            return res;
        }

        public async Task<bool> AddPlayerToGameStreamAsync(string gameGUID, string playerGUID, StreamWriter playerWriter)
        {
            var res = await Task.Run(() =>
            {
                Guid GameGUID = new(gameGUID);
                Guid PlayerGUID = new(playerGUID);

                var targetGame = ActiveGames.FirstOrDefault(game => game.Id == GameGUID);

                if (targetGame == null)
                    return false;

                if (targetGame.PlayerConnections.ContainsKey(PlayerGUID.ToString()))
                    return false;
                else
                    targetGame.PlayerConnections.Add(playerGUID, playerWriter);

                return true;
            });

            return res;
        }
    }
}