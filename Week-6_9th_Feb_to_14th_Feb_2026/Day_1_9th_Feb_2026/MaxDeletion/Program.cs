using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter the string:");
        string s = Console.ReadLine();

        int maxDeletions = s.Length / 2;

        Console.WriteLine("Maximum deletions possible:");
        Console.WriteLine(maxDeletions);
    }
}
