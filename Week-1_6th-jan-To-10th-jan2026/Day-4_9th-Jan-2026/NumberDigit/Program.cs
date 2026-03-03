namespace NumberDigit
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int number = Convert.ToInt32(Console.ReadLine());
            int output1;

            if (number < 0)
                output1 = -1;
            else
            {
                int count = 0;
                do
                {
                    count++;
                    number /= 10;
                } while (number != 0);

                output1 = count;
            }

            Console.WriteLine(output1);
        }
    }
    }

