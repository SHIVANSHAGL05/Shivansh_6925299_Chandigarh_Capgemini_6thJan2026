namespace SquareofOdd
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
                int sum = 0;

                while (input1 > 0)
                {
                    int d = input1 % 10;
                    if (d % 2 != 0)
                        sum += d * d;
                    input1 /= 10;
                }

                output = sum;
            }

            Console.WriteLine(output);
        }
    }
}
