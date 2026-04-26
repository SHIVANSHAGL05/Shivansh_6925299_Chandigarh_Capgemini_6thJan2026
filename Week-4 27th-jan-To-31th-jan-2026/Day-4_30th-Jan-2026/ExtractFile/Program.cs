using System;

class Program
{
    static void Main()
    {
        string file = Console.ReadLine();
        Console.WriteLine(file.Substring(file.LastIndexOf('.') + 1));
    }
}
