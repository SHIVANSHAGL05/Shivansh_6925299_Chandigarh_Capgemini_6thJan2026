using System;
class Program
{
    static void Main()
    {
        Console.WriteLine("Enter a positive integer:");
        int num = Convert.ToInt32(Console.ReadLine());

        int root = (int)Math.Round(Math.Sqrt(num));
        int closestSquare = root * root;

        Console.WriteLine("Closest integer having a whole number square root:");
        Console.WriteLine(closestSquare);
    }
}
