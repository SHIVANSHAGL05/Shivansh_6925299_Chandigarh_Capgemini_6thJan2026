namespace RemoveRepeated
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] input = { 1, 2, 2, 3, 4, 4, 5 };

            foreach (int n in input)
            {
                if (n < 0)
                {
                    Console.WriteLine("-1");
                    return;
                }
            }

            int[] output = new int[input.Length];
            int index = 0;

            for (int i = 0; i < input.Length; i++)
            {
                bool isDuplicate = false;

                for (int j = 0; j < index; j++)
                {
                    if (input[i] == output[j])
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                    output[index++] = input[i];
            }

            for (int i = 0; i < index; i++)
                Console.Write(output[i] + " ");
        }
    }
}
