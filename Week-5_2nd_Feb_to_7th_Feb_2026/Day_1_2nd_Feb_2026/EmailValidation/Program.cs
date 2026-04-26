using System.Text.RegularExpressions;

namespace EmailValidation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter email:");
            string email = Console.ReadLine();

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (Regex.IsMatch(email, pattern))
                Console.WriteLine("Output: Valid");
            else
                Console.WriteLine("Output: Invalid");
        }
    }
}
