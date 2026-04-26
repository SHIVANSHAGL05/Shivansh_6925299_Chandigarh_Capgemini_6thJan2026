using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter a positive integer:");
        string input = Console.ReadLine();

        int sum = 0;

        for (int i = 0; i < input.Length; i++)
        {
            sum += input[i] - '0';
        }

        Console.WriteLine("Sum of digits: " + sum);
    }
}
