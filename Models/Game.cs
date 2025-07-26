using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        private List<Guid> PlayerIDs { get; set; } = [];
        private List<string> playerUsernames { get; set; } = [];
        [JsonIgnore]
        public Dictionary<string, StreamWriter> PlayerConnections { get; } = [];
        public GameState State { get; set; } = GameState.WAITING_FOR_PLAYERS;
        public Guid? HostId { get; set; } = Guid.Empty;
        public Guid? WinnerID { get; set; } = Guid.Empty;

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

            PlayerIDs.Add(newPlayer);
            playerUsernames.Add(username);
            return true;
        }
        public bool RemovePlayer(Guid playerId)
        {
            var targetPlayer = PlayerIDs.FirstOrDefault(playerGuid => playerGuid == playerId);
            // if (targetPlayer == null) return false;
            PlayerIDs.Remove(targetPlayer);
            return true;
        }

        public async Task SendToAllExcept(string playerGUID, string content)
        {
            foreach (var playerConn in PlayerConnections)
            {
                if (playerConn.Key != playerGUID)
                {
                    await playerConn.Value.WriteLineAsync(content);
                    await playerConn.Value.FlushAsync();
                }
            }
        }

        public async Task SendPlayersAlreadyInGame(string playerGUID, string content)
        {
            var target_player = PlayerConnections.FirstOrDefault(playerConn => playerConn.Key == playerGUID);
            if (target_player.Value == null)
                return;
            await target_player.Value.WriteLineAsync(content);
        }
    }
}