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
        private List<Player> Players { get; set; } = [];
        [JsonIgnore]
        public Dictionary<string, StreamWriter> PlayerConnections { get; } = [];
        public GameState State { get; set; } = GameState.WAITING_FOR_PLAYERS;
        public Guid? HostId { get; set; } = Guid.Empty;
        public Guid? WinnerID { get; set; } = Guid.Empty;

        public ReadOnlyCollection<Player> GetAllPlayers()
        {
            return Players.AsReadOnly();
        }

        public bool AddPlayer(Player newPlayer)
        {
            if (Players.Count >= MaxPlayers)
                return false;

            Players.Add(newPlayer);
            return true;
        }
        public bool RemovePlayer(Guid playerId)
        {
            var targetPlayer = Players.Find(player => player.Id == playerId);
            if (targetPlayer == null) return false;
            Players.Remove(targetPlayer);
            return true;
        }
    }
}