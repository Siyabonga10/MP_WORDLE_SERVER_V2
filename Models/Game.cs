using System.Collections.ObjectModel;
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
        [JsonIgnore]
        public Dictionary<string, StreamWriter> PlayerConnections { get; } = [];
        public GameState State { get; set; } = GameState.WAITING_FOR_PLAYERS;
        public Guid? HostId { get; set; } = Guid.Empty;
        public Guid? WinnerID { get; set; } = Guid.Empty;

        public ReadOnlyCollection<Guid> GetAllPlayers()
        {
            return PlayerIDs.AsReadOnly();
        }

        public bool AddPlayer(Guid newPlayer)
        {
            if (PlayerIDs.Count >= MaxPlayers)
                return false;

            PlayerIDs.Add(newPlayer);
            return true;
        }
        public bool RemovePlayer(Guid playerId)
        {
            var targetPlayer = PlayerIDs.FirstOrDefault(playerGuid => playerGuid == playerId);
           // if (targetPlayer == null) return false;
            PlayerIDs.Remove(targetPlayer);
            return true;
        }
    }
}