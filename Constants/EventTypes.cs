namespace MP_WORDLE_SERVER_V2.Constants
{
    public static class EventTypes
    {
        public const string PlayersInGame = "PlayersInGame";
        public const string PlayerJoined = "PlayerJoined";
        public const string StartGame = "StartGame";
        public const string WinnerUpdate = "WinnerUpdate";
    }

    public class TmpClass
    {
        public static string Tmp { get; set; } = "Un-initiased";
    }
}