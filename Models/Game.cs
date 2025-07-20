using System.Collections.ObjectModel;

namespace MP_WORDLE_SERVER_V2.Models
{
    public class Game(int gameId)
    {
        readonly private static int MaxPlayers = 5;
        public int Id { get; set; } = gameId;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
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
        public bool RemovePlayer(int playerId)
        {
            var targetPlayer = players.Find(player => player.Id == playerId);
            if (targetPlayer == null) return false;
            players.Remove(targetPlayer);
            return true;
        }
    }
}