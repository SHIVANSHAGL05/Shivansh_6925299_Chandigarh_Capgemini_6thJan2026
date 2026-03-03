namespace CheckIfAllWordsAreAnagrams
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter comma separated words:");
            string[] words = Console.ReadLine().Split(',');

            string baseWord = String.Concat(words[0].OrderBy(c => c));
            bool isAnagram = true;

            foreach (string word in words)
            {
                if (String.Concat(word.OrderBy(c => c)) != baseWord)
                {
                    isAnagram = false;
                    break;
                }
            }

            Console.WriteLine("Are all words anagrams? " + isAnagram);
        }
    }
}


// testcase : dusty,study ==> true (not give space)
