using System.Collections.ObjectModel;

namespace MP_WORDLE_SERVER_V2.Models
{
    public class Game(int gameId)
    {
        public int Id { get; set; } = gameId;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        readonly private List<Player> players = [];

        public ReadOnlyCollection<Player> GetAllPlayers()
        {
            return players.AsReadOnly();
        }
    }
}