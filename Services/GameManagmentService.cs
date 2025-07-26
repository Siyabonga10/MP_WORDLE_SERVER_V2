using System.Text.Json;
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

        public async Task<bool> AddPlayerToGameAsync(string gameId, string playerId, string username, bool ishost)
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
                Guid GameGUID = new(gameGUID);
                Guid PlayerGUID = new(playerGUID);

                var targetGame = ActiveGames.FirstOrDefault(game => game.Id == GameGUID);

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

        public async Task SendToAllExcept(string gameID, string playerGUID, string content)
        {
            Guid gameGUID = new(gameID);
            var target_game = ActiveGames.FirstOrDefault(game => game.Id == gameGUID);
            if (target_game == null)
                return;

            await target_game.SendToAllExcept(playerGUID, content);
        }

        public async Task SendPlayersAlreadyInGame(string targetPlauerGuid, string gameID, string username)
        {
            Guid gameGUID = new(gameID);
            var target_game = ActiveGames.FirstOrDefault(game => game.Id == gameGUID);
            if (target_game == null)
                return;


            foreach (var player in target_game.GetPlayerUsernames())
                Console.WriteLine($"Player in game {player}");
            var player_usernames = target_game.GetPlayerUsernames().Except([username]);
            var payload = string.Join("\n", player_usernames);
            Console.WriteLine($"Receipient {username}\nData: {payload}");
            await target_game.SendPlayersAlreadyInGame(targetPlauerGuid, payload);
        }
    }
}