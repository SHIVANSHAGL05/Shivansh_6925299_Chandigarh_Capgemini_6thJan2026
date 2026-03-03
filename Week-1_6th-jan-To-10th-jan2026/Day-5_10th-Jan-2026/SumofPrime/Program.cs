namespace SumofPrime
{
    internal class Program
    {

        static bool IsPrime(int n)
        {
            if (n <= 1) return false;
            for (int i = 2; i <= n / 2; i++)
                if (n % i == 0) return false;
            return true;
        }
        static void Main(string[] args)
        {
            int[] input1 = { 1, 2, 3, 4, 5 };
            int input2 = 5;
            int output1 = 0;
            bool found = false;

            if (input2 < 0)
                output1 = -2;
            else
            {
                for (int i = 0; i < input2; i++)
                {
                    if (input1[i] < 0)
                    {
                        output1 = -1;
                        Console.WriteLine(output1);
                        return;
                    }
                    if (IsPrime(input1[i]))
                    {
                        output1 += input1[i];
                        found = true;
                    }
                }

                if (!found)
                    output1 = -3;
            }

            Console.WriteLine(output1);
        }
    }
}
