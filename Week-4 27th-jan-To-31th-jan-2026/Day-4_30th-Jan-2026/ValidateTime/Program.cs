using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string time = Console.ReadLine();
        string pattern = @"^(0[1-9]|1[0-2]):[0-5][0-9]\s?(am|pm)$";

        Console.WriteLine(Regex.IsMatch(time, pattern, RegexOptions.IgnoreCase)
            ? "Valid time format"
            : "Invalid time format");
    }
}
