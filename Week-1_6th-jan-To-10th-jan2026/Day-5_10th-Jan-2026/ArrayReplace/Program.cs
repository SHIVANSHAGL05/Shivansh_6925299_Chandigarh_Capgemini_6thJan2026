namespace ArrayReplace
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] input1 = { 12, 34, 56, 17, 2 };
            int input2 = 5;
            int[] output1 = new int[input2];

            if (input2 < 0)
            {
                output1[0] = -2;
            }
            else if (input2 % 2 == 0)
            {
                output1[0] = -3;
            }
            else
            {
                for (int i = 0; i < input2; i++)
                {
                    if (input1[i] < 0)
                    {
                        output1[0] = -1;
                        Console.WriteLine(output1[0]);
                        return;
                    }
                    output1[i] = input1[input2 - 1 - i];
                }
            }

            for (int i = 0; i < output1.Length; i++)
                Console.Write(output1[i] + " ");
        }
    }
}
