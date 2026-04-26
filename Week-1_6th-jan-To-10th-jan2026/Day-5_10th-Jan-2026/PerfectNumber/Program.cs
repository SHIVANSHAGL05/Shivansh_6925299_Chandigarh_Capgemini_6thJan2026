using System;

namespace PerfectNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            int sum = 0;
            int output;

            if (n < 0)
                output = -2;
            else
            {
                for (int i = 1; i < n; i++)
                    if (n % i == 0)
                        sum += i;

                output = (sum == n) ? 1 : -1;
            }

            Console.WriteLine(output);
        }
    }
}
