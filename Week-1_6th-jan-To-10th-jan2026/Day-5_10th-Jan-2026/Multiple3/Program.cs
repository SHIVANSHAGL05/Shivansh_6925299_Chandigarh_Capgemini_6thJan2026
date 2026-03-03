namespace Multiple3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] input1 = { 1, 2, 3, 4, 5, 6 };
            int input2 = 6;
            int output = 0;

            if (input2 < 0)
                output = -2;
            else
            {
                for (int i = 0; i < input2; i++)
                {
                    if (input1[i] < 0)
                    {
                        output = -1;
                        break;
                    }
                    if (input1[i] % 3 == 0)
                        output++;
                }
            }

            Console.WriteLine(output);
        }
    }
}
