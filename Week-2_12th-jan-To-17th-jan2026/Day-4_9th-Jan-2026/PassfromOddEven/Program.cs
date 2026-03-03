namespace PassfromOddEven
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 1, 2, 3, 4, 5 };
            int output;

            if (arr.Length < 0)
            {
                output = -2;
            }
            else
            {
                int oddSum = 0, evenSum = 0;

                foreach (int n in arr)
                {
                    if (n < 0)
                    {
                        Console.WriteLine("-1");
                        return;
                    }

                    if (n % 2 == 0)
                        evenSum += n;
                    else
                        oddSum += n;
                }

                output = (oddSum + evenSum) / 2;
            }

            Console.WriteLine(output);
        }
    }
}
