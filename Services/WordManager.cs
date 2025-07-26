using System.Security.Cryptography;

namespace MP_WORDLE_SERVER_V2.Services
{
    public interface IWordManager
    {
        public List<string> GetRandomWords(int count);
    }

    public class TestWordManager : IWordManager
    {
        private static int MAX_WORDS = 20;
        public List<string> GetRandomWords(int count)
        {
            if (count > MAX_WORDS)
                return [];

            List<string> Words = [];
            for (int i = 0; i < count; i++)
                Words.Add($"RandWord: {i}");

            return Words;
        }
    }
}