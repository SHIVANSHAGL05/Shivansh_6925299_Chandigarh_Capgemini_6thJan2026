namespace RemoveDuplicate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] input1 = { 1, 2, 2, 3, 3 };
            int input2 = 5;

            if (input2 < 0)
            {
                Console.WriteLine(-2);
                return;
            }

            for (int i = 0; i < input2; i++)
            {
                if (input1[i] < 0)
                {
                    Console.WriteLine(-1);
                    return;
                }
            }

            int[] temp = new int[input2];
            int k = 0;

            for (int i = 0; i < input2; i++)
            {
                bool found = false;
                for (int j = 0; j < k; j++)
                    if (temp[j] == input1[i])
                        found = true;

                if (!found)
                    temp[k++] = input1[i];
            }

            for (int i = 0; i < k; i++)
                Console.Write(temp[i] + " ");
        }
    }
}
