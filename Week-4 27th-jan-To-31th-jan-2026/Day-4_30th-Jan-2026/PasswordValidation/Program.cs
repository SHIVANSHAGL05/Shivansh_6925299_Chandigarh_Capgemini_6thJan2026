using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string pwd = Console.ReadLine();
        string pattern = @"^(?![0-9\W])(?=.*[@#_])(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@#_]{8,}(?<!\W)$";

        Console.WriteLine(Regex.IsMatch(pwd, pattern) ? 1 : -1);
    }
}
