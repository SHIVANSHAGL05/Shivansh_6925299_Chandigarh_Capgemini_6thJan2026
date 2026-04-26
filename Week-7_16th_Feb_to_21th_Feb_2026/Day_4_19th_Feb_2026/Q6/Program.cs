namespace Q6
{
    using System;

    class Program
    {
        static void Main()
        {
            int sem = int.Parse(Console.ReadLine());

            for (int i = 1; i <= sem; i++)
            {
                int sub = int.Parse(Console.ReadLine());
                int max = 0;

                for (int j = 0; j < sub; j++)
                {
                    int m = int.Parse(Console.ReadLine());
                    if (m < 0 || m > 100)
                    {
                        Console.WriteLine("You have entered invalid mark.");
                        return;
                    }
                    if (m > max) max = m;
                }
                Console.WriteLine($"Maximum mark in {i} semester:{max}");
            }
        }
    }

}
