using System;
using System.Text.RegularExpressions;
class Program
{
    static void Main()
    {
        string text = Console.ReadLine();
        string pattern = @"#\w+";

        MatchCollection matches = Regex.Matches(text, pattern);

        foreach (Match m in matches)
            Console.WriteLine(m.Value);

    }
}