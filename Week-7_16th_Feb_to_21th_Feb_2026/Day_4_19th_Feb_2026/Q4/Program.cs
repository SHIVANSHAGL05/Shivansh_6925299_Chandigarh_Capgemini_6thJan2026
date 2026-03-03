using System;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());

        for (int i = 0; i < n; i++)
        {
            string[] s = Console.ReadLine().Split();
            int cars = int.Parse(s[0]);
            int bikes = int.Parse(s[1]);
            Console.WriteLine(cars * 4 + bikes * 2);
        }
    }
}
