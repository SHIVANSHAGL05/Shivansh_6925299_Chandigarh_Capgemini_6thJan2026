namespace Input3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int input1 = 10, input2 = 5, input3 = 3;
            int output;

            if (input1 < 0 && input2 < 0)
                output = -1;
            else
            {
                switch (input3)
                {
                    case 1: output = input1 + input2; break;
                    case 2: output = input1 - input2; break;
                    case 3: output = input1 * input2; break;
                    case 4: output = input1 / input2; break;
                    default: output = 0; break;
                }
            }

            Console.WriteLine(output);
        }
    }
}
