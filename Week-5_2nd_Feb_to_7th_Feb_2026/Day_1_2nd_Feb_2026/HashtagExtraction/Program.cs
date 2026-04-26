using System.Text.RegularExpressions;

namespace HashtagExtraction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter text:");
            string input = Console.ReadLine();

            string pattern = @"#\w+";

            MatchCollection matches = Regex.Matches(input, pattern);

            Console.WriteLine("Output:");
            foreach (Match m in matches)
            {
                Console.WriteLine(m.Value);
            }
        }
    }
}

// testcase :: Loving the vibes! #Travel #Adventure #2026Goals