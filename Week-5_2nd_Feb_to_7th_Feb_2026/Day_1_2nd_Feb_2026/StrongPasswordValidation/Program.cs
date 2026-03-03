using System.Text.RegularExpressions;

namespace StrongPasswordValidation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();

            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$";

            if (Regex.IsMatch(password, pattern))
                Console.WriteLine("Output: Strong");
            else
                Console.WriteLine("Output: Weak");
        }
    }
}
