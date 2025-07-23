namespace MP_WORDLE_SERVER_V2.Models
{
    public class GamePlayerJunction()
    {
        public int Id { get; set; } = 0;
        public Guid GameId { get; set; } = Guid.Empty;
        public Game? Game { get; set; } = null;
        public Guid PlayerId { get; set; } = Guid.Empty;
        public Player? Player { get; set; } = null;

    }
}