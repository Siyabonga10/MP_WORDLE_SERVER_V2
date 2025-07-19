namespace MP_WORDLE_SERVER_V2.Models
{
    public class Player(int id, string username)
    {
        public int Id { get; set; } = id;
        public string Username { get; set; } = username;
    }
}