using System;

namespace SumEven
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int number = Convert.ToInt32(Console.ReadLine());
            int output;

            if (number < 0)
                output = -1;
            else if (number > 32767)
                output = -2;
            else
            {
                int sum = 0;
                while (number > 0)
                {
                    int d = number % 10;
                    if (d % 2 == 0)
                        sum += d;
                    number /= 10;
                }
                output = sum;
            }

            Console.WriteLine(output);
        }
    }
}
