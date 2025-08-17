using System.Security.Cryptography;

namespace MP_WORDLE_SERVER_V2.Services
{
    public interface IWordManager
    {
        public Task<List<string>> GetRandomWords(int count);
    }

    public class TestWordManager : IWordManager
    {
        private static int MAX_WORDS = 20;
        private static List<string> allWords = [];
        public async Task<List<string>> GetRandomWords(int count)
        {
            if (count > MAX_WORDS)
                return [];

            if (allWords.Count == 0)
            {
                var reader = new StreamReader("words.txt");
                string? word;
                while ((word = await reader.ReadLineAsync()) != null)
                {
                    allWords.Add(word);
                }
            }

            List<string> Words = [];

            // Not ideal but eyyy
            // TODO: Generate the words in a less crazy way
            var rng = new Random();
            for (int i = 0; i < count; i++)
            {
                var newIndex = rng.Next(allWords.Count);
                while (Words.Contains(allWords[newIndex]))
                    newIndex = rng.Next(allWords.Count);
                Words.Add(allWords[newIndex]);
            }

            return Words;
        }
    }
}