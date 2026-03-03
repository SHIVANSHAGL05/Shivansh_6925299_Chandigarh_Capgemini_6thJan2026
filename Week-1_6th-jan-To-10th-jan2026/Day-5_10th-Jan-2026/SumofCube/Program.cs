namespace SumofCube
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

        static void Main()
        {
            int n = Convert.ToInt32(Console.ReadLine());
            int output = 0;

            if (n < 0 || n > 7)
                output = -1;
            else
            {
                for (int i = 1; i <= n; i++)
                    if (IsPrime(i))
                        output += i * i * i;
            }

            Console.WriteLine(output);
        }
    }
}
