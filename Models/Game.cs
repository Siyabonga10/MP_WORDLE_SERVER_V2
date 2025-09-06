using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using MP_WORDLE_SERVER_V2.Constants;

namespace MP_WORDLE_SERVER_V2.Models
{
    public enum GameState {WAITING_FOR_PLAYERS, ON_GOING, PROCESSING_RESULTS, COMPLETE}
    public class Game
    {
        public Game(Guid Id)
        {
            this.Id = Id;
        }
        readonly private static int MaxPlayers = 5;
        public Guid Id { get; set; } = Guid.Empty;
        [NotMapped]
        public string ShortId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        private List<Guid> PlayerIDs { get; set; } = [];
        private List<string> playerUsernames { get; set; } = [];
        [JsonIgnore]
        public Dictionary<string, StreamWriter> PlayerConnections { get; } = [];
        public GameState State { get; set; } = GameState.WAITING_FOR_PLAYERS;
        public Guid? HostId { get; set; } = Guid.Empty;
        public Guid? WinnerID { get; set; } = Guid.Empty;
        private Task? gameTask;

        // Result managment for post game
        private Dictionary<string, int> results = [];

        public ReadOnlyCollection<Guid> GetAllPlayers()
        {
            return PlayerIDs.AsReadOnly();
        }

        public ReadOnlyCollection<string> GetPlayerUsernames()
        {
            return playerUsernames.AsReadOnly();
        }

        public bool AddPlayer(Guid newPlayer, string username)
        {
            if (PlayerIDs.Count >= MaxPlayers)
                return false;

            if (playerUsernames.Any(_username => _username == username))
                return true; // Avoid adding the same player multiple times, but technically they are in the list
            PlayerIDs.Add(newPlayer);
            playerUsernames.Add(username);
            results.Add(username, -1);
            return true;
        }
        public bool RemovePlayer(Guid playerId)
        {
            var targetPlayer = PlayerIDs.FirstOrDefault(playerGuid => playerGuid == playerId);
            // if (targetPlayer == null) return false;
            PlayerIDs.Remove(targetPlayer);
            return true;
        }

        public async Task SendToAllExcept(string playerGUID, string type, string content)
        {
            var data = $"event: {type}\r\ndata: {content}\r\n\r\n";
            foreach (var playerConn in PlayerConnections)
            {
                if (playerConn.Key != playerGUID)
                {
                    await playerConn.Value.WriteAsync(data);
                    await playerConn.Value.FlushAsync();
                }
            }
        }

        public async Task SendPlayersAlreadyInGame(string playerGUID, string type, IEnumerable<string> usernames)
        {
            var target_player = PlayerConnections.FirstOrDefault(playerConn => playerConn.Key == playerGUID);
            if (target_player.Value == null)
                return;
            var data = $"event: {type}\r\n";
            foreach (var username in usernames)
                data += $"data: {username}\r\n";

            data += "\r\n";
            await target_player.Value.WriteAsync(data);
            await target_player.Value.FlushAsync();
        }

        public async Task SendToAll(string type, string content)
        {
            var data = $"event: {type}\r\n{content}\r\n\r\n";

            foreach (var playerConn in PlayerConnections)
            {
                await playerConn.Value.WriteAsync(data);
                await playerConn.Value.FlushAsync();
            }
        }

        public async Task EndGame()
        {
            State = GameState.COMPLETE;
            var data = $"event: {EventTypes.WinnerUpdate}\r\n";

            foreach (var result in results)
                data += $"{result.Key}:{result.Value}\r\n";

            data += "\r\n";
            foreach (var playerConn in PlayerConnections)
            {
                await playerConn.Value.WriteAsync(data);
                await playerConn.Value.FlushAsync();
            }
        }


        public bool AddResult(string username, int score)
        {
            if (results.TryGetValue(username, out int current_score))
                if (current_score != -1) return false;
            var added = results[username] =  score;
            if (!results.ContainsValue(-1))
            {
                _ = Task.Run(async () =>
                {
                    if (gameTask != null && !gameTask.IsCompleted)
                        gameTask.Dispose();
                    State = GameState.COMPLETE;
                    await EndGame();
                });
            }
            return true;
        }
        public void StartGame()
        {
            State = GameState.ON_GOING;
            gameTask = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(2.1));
                if (State == GameState.ON_GOING)
                    await EndGame();
            });
        }
    }
}