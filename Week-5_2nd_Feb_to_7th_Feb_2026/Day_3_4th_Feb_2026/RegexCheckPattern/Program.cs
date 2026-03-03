using System.Text.RegularExpressions;

namespace RegexCheckPattern
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the message in format:");
            Console.WriteLine("Hi how are you Dear <Name>");

            string input = Console.ReadLine();

            string pattern = @"^Hi how are you Dear\s[A-Za-z]{16,}$";

            if (Regex.IsMatch(input, pattern))
            {
                Console.WriteLine("Message is valid");
                Console.WriteLine("Your input is: " + input);
            }
            else
            {
                Console.WriteLine("Invalid message");
                Console.WriteLine("Make sure name has more than 15 characters.");
            }
        }
    }
}
