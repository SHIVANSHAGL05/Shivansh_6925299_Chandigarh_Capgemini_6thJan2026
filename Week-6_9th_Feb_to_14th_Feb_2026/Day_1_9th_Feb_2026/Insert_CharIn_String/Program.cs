using System;

class Program
{
    static void Main()
    {
        Console.Write("Enter a string:");
        string input = Console.ReadLine();

        Console.Write("Enter a character to insert:");
        char ch = Convert.ToChar(Console.ReadLine());

        Console.Write("Enter the position (1-based):");
        int position = Convert.ToInt32(Console.ReadLine());

        if (position < 1 || position > input.Length + 1)
        {
            Console.WriteLine("Invalid position");
            return;
        }

        string result = input.Insert(position - 1, ch.ToString());

        Console.Write("Resulting string:");
        Console.WriteLine(result);
    }
}
