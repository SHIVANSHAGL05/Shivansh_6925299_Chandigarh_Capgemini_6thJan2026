namespace productdivisibility
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int input1 = 56;
            int output;

            if (input1 < 0)
                output = -1;
            else if (input1 % 3 == 0 || input1 % 5 == 0)
                output = -2;
            else
            {
                int product = 1;
                while (input1 > 0)
                {
                    product *= input1 % 10;
                    input1 /= 10;
                }
                output = (product % 3 == 0 || product % 5 == 0) ? 1 : 0;
            }

            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}