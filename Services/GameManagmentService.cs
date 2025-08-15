using MP_WORDLE_SERVER_V2.Models;

namespace MP_WORDLE_SERVER_V2.Services
{
    public class GameManagementService
    {
        private readonly List<Game> ActiveGames = [];
        public Game CreateGame()
        {
            var newGame = new Game(Guid.NewGuid());
            newGame.ShortId = newGame.Id.ToString()[..5];
            ActiveGames.Add(newGame);
            return newGame;
        }

        public async Task<bool> AddPlayerToGameAsync(string gameId, string playerId, string username, bool ishost)
        {
            var res = await Task.Run(() =>
            {
                Guid playerGUID = new(playerId);

                var targetGame = ActiveGames.FirstOrDefault(game => game.ShortId == gameId);

                if (targetGame == null)
                    return false;

                if (targetGame.State == GameState.WAITING_FOR_PLAYERS)
                {
                    var playerAdded = targetGame.AddPlayer(playerGUID, username);
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
                Guid PlayerGUID = new(playerGUID);

                var targetGame = ActiveGames.FirstOrDefault(game => game.ShortId == gameGUID);
                if (targetGame == null)
                    return false;
                var playerInGame = targetGame.GetAllPlayers().Any(playerGuid => playerGuid == PlayerGUID);

                if (targetGame.PlayerConnections.ContainsKey(PlayerGUID.ToString()) || !playerInGame)
                    return false;
                else
                    targetGame.PlayerConnections.Add(playerGUID, playerWriter);

                return true;
            });

            return res;
        }

        public async Task SendToAllExcept(string gameID, string playerGUID, string content, string type)
        {
            var target_game = ActiveGames.FirstOrDefault(game => game.ShortId == gameID);
            if (target_game == null)
                return;

            await target_game.SendToAllExcept(playerGUID, type, content);
        }

        public async Task SendPlayersAlreadyInGame(string targetPlauerGuid, string gameID, string username, string type)
        {
            Guid currentPlayerGuid = new(targetPlauerGuid);
            var target_game = ActiveGames.FirstOrDefault(game => game.ShortId == gameID);
            if (target_game == null)
                return;

            if (target_game.HostId == currentPlayerGuid)
                return;
            var player_usernames = target_game.GetPlayerUsernames().Except([username]);
            var payload = string.Join("\r\n", player_usernames);
            
            await target_game.SendPlayersAlreadyInGame(targetPlauerGuid, type, payload);
        }

        public bool StartGame(string playerID, string gameID)
        {
            Guid playerGUID = new(playerID);
            var target_game = ActiveGames.FirstOrDefault(game => game.ShortId == gameID);
            if (target_game == null)
                return false;

            if (target_game.State != GameState.WAITING_FOR_PLAYERS)
                return false;
            if (target_game.HostId != playerGUID)
                return false;
            target_game.State = GameState.ON_GOING;
            return true;
        }

        public async Task SendToAll(string gameID, string eventType, string content)
        {
            var target_game = ActiveGames.FirstOrDefault(game => game.ShortId == gameID);
            if (target_game == null)
                return;
            await target_game.SendToAll(eventType, content);
        }
    }
}