namespace AvgNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int input1 = Convert.ToInt32(Console.ReadLine());
            int output;

            if (input1 < 0)
            {
                output = -1;
            }
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

                output = (count > 0) ? sum / count : 0;
            }

            Console.WriteLine(output);
        }
    }
}
