namespace EvenOdd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 1, 2, 3, 4, 5, 6 };
            int output1;

            if (arr.Length < 0)
                output1 = -2;
            else
            {
                int evenSum = 0, oddSum = 0;

                foreach (int n in arr)
                {
                    if (n < 0)
                    {
                        output1 = -1;
                        Console.WriteLine(output1);
                        return;
                    }

                    if (n % 2 == 0)
                        evenSum += n;
                    else
                        oddSum += n;
                }

                output1 = (evenSum + oddSum) / 2;
            }

            Console.WriteLine(output1);
        }
    }
}
