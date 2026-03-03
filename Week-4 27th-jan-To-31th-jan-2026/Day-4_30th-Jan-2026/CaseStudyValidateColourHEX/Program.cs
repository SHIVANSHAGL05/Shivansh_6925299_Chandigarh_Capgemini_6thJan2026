using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string color = Console.ReadLine();
        string pattern = @"^#[0-9A-Fa-f]{6}$";

        Console.WriteLine(Regex.IsMatch(color, pattern) ? "Valid" : "Invalid");
    }
}
