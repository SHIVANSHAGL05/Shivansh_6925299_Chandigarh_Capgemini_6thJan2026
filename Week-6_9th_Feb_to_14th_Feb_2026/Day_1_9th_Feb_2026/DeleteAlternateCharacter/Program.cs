using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter a string:");
        string input = Console.ReadLine();

        string result = "";

        for (int i = 0; i < input.Length; i++)
        {
            if (i % 2 == 0)
            {
                result += input[i];
            }
        }

        Console.WriteLine("Result:");
        Console.WriteLine(result);
    }
}
