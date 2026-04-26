namespace SearchElement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] input1 = { 1, 2, 2, 3, 3,3 };
            int input2 = input1.Length;
            int input3 = 3;
            int output = 0;

            if (input2 < 0)
            {
                output = -2;
            }
            else if (input3 < 0)
            {
                output = -3;
            }
            else
            {
                foreach (int n in input1)
                {
                    if (n < 0)
                    {
                        output = -1;
                        Console.WriteLine(output);
                        return;
                    }

                    if (n == input3)
                        output++;
                }
            }

            Console.WriteLine(output);
        }
    }
}
