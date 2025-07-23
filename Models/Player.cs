namespace MP_WORDLE_SERVER_V2.Models
{
    public class Player(Guid id, string username)
    {
        public Guid Id { get; set; } = id;
        public string Username { get; set; } = username;
    }
}