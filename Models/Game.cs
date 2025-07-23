using System.Collections.ObjectModel;

namespace MP_WORDLE_SERVER_V2.Models
{
    public class Game
    {
        public Game(Guid Id)
        {
            this.Id = Id;
        }
        readonly private static int MaxPlayers = 5;
        public Guid Id { get; set; } = Guid.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        readonly private List<Player> players = [];

        public ReadOnlyCollection<Player> GetAllPlayers()
        {
            return players.AsReadOnly();
        }

        public bool AddPlayer(Player newPlayer)
        {
            if (players.Count >= MaxPlayers)
                return false;

            players.Add(newPlayer);
            return true;
        }
        public bool RemovePlayer(Guid playerId)
        {
            var targetPlayer = players.Find(player => player.Id == playerId);
            if (targetPlayer == null) return false;
            players.Remove(targetPlayer);
            return true;
        }
    }
}