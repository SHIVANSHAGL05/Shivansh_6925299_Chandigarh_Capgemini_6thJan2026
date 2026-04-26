namespace SumofCube
{
    internal class Program
    {
        static bool IsPrime(int n)
        {
            if (n < 2)
                return false;

            for (int i = 2; i <= Math.Sqrt(n); i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }

        static void Main()
        {
            int input1 = Convert.ToInt32(Console.ReadLine());
            int output;

            if (input1 < 0)
            {
                output = -1;
            }
            else if (input1 > 32767)
            {
                output = -2;
            }
            else
            {
                int sum = 0;

                for (int i = 1; i <= input1; i++)
                {
                    if (IsPrime(i))
                    {
                        sum += i * i * i;
                    }
                }

                output = sum;
            }

            Console.WriteLine(output);
        }
    }
}
