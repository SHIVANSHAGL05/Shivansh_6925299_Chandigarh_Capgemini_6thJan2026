namespace SumofCube
{
    internal class Program
    {
        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            for (int i = 2; i <= Math.Sqrt(n); i++)
                if (n % i == 0) return false;
            return true;
        }
        static void Main(string[] args)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            int output;

            if (n < 0)
                output = -1;
            else if (n > 32676)
                output = -2;
            else
            {
                int sum = 0;
                for (int i = 1; i <= n; i++)
                    if (IsPrime(i))
                        sum += i * i * i;

                output = sum;
            }

            Console.WriteLine(output);
        }
    }
}
