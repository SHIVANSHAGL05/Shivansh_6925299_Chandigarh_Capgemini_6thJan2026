namespace SumofFactor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int input1 = 3, input2 = 15;
            int output = 0;

            if (input1 < 0)
                output = -1;
            else if (input2 > 32627)
                output = -2;
            else
            {
                for (int i = input1; i <= input2; i += input1)
                    output += i;
            }

            Console.WriteLine(output);
        }
    }
}
