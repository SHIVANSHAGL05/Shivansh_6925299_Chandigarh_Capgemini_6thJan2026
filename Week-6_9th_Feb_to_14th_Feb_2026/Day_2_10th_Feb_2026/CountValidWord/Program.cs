namespace CountValidWord
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string:");
            string input = Console.ReadLine();

            string[] words = input.Split(' ');

            int count = 0;

            foreach (string word in words)
            {
                if (IsValid(word))
                    count++;
            }

            Console.WriteLine("Number of valid words: " + count);
        }

        static bool IsValid(string word)
        {
            if (word.Length <= 2)
                return false;

            bool hasVowel = false;
            bool hasConsonant = false;

            foreach (char c in word)
            {
                if (!char.IsLetterOrDigit(c))
                    return false;

                if (char.IsLetter(c))
                {
                    char ch = char.ToLower(c);

                    if (ch == 'a' || ch == 'e' || ch == 'i' || ch == 'o' || ch == 'u')
                        hasVowel = true;
                    else
                        hasConsonant = true;
                }
            }

            return hasVowel && hasConsonant;
        }
    }
}


/*
 * testcase : hello sky a1b a@b
 *  output : 2
 */