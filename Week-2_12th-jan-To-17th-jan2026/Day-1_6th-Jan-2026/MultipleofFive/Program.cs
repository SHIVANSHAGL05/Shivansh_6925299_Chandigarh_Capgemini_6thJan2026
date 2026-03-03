namespace MultipleofFive
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 5, 10, 15, 20 };
            int output;

            if (arr.Length < 0)
            {
                output = -2;
            }
            else
            {
                int sum = 0, count = 0;

                foreach (int n in arr)
                {
                    if (n < 0)
                    {
                        Console.WriteLine("-1");
                        return;
                    }

                    if (n % 5 == 0)
                    {
                        sum += n;
                        count++;
                    }
                }

                output = (count > 0) ? sum / count : 0;
            }

            Console.WriteLine(output);
        }
    }
}
