namespace AvgofFive
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int input1 = Convert.ToInt32(Console.ReadLine());
            int output;

            if (input1 < 0)
                output = -1;
            else if (input1 > 500)
                output = -2;
            else
            {
                int sum = 0, count = 0;

                for (int i = 1; i <= input1; i++)
                {
                    if (i % 5 == 0)
                    {
                        sum += i;
                        count++;
                    }
                }

                output = sum / count;
            }

            Console.WriteLine(output);
        }
    }
}
