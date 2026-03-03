using System.Text.RegularExpressions;

namespace DateExtraction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter text:");
            string input = Console.ReadLine();

            string pattern = @"\b\d{2}/\d{2}/\d{4}\b";

            MatchCollection matches = Regex.Matches(input, pattern);

            Console.WriteLine("Output:");
            foreach (Match m in matches)
            {
                Console.WriteLine(m.Value);
            }
        }
    }
}


// testcase : Our trip is on 15/02/2026 and return on 25/02/2026.