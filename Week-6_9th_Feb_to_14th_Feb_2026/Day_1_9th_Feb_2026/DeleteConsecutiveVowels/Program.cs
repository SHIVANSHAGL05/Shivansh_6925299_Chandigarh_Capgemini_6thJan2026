using System.Text.RegularExpressions;

namespace DeleteConsecutiveVowels
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a string :: ");
            string st = Console.ReadLine().ToLower();

            MatchCollection vowels = Regex.Matches(st, "[aeiou]{2}");
            Console.WriteLine("Maximum consecutive vowels delete :: " + vowels.Count);
            Console.ReadLine();
        }
    }
}
