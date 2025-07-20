namespace MP_WORDLE_SERVER_V2.Models
{
    public class GamePlayerJunction()
    {
        public int Id { get; set; } = 0;
        public int GameId { get; set; } = 0;
        public Game? Game { get; set; } = null;
        public int PlayerId { get; set; } = 0;
        public Player? Player { get; set; } = null;

    }
}