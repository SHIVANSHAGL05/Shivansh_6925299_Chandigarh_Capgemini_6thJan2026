namespace multiplyarray
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] input1 = { 1, 2, 3, 4, 5 };
            int[] input2 = { 9, 8, 7, 6, 5 };
            int[] output = new int[input1.Length];

            if (input1.Length < 0 || input2.Length < 0)
            {
                output[0] = -2;
            }
            else
            {
                foreach (int n in input1)
                    if (n < 0) { output[0] = -1; goto end; }

                foreach (int n in input2)
                    if (n < 0) { output[0] = -1; goto end; }

                Array.Sort(input1);
                Array.Sort(input2);
                Array.Reverse(input2);

                for (int i = 0; i < input1.Length; i++)
                    output[i] = input1[i] * input2[i];
            }

        end:
            foreach (int n in output)
                Console.Write(n + " ");
        }
    }
}
