using System.Text.RegularExpressions;

namespace PhoneNumberExtraction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter text:");
            string input = Console.ReadLine();

            string pattern = @"\b\d{10}\b";

            MatchCollection matches = Regex.Matches(input, pattern);

            Console.WriteLine("Output:");
            foreach (Match m in matches)
            {
                Console.WriteLine(m.Value);
            }
        }
    }
}
