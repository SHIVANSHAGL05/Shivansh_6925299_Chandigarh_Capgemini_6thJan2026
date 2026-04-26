namespace DecimaltoBinary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int input1 = 10;

            if (input1 < 0)
            {
                Console.WriteLine("-1");
                return;
            }

            int[] bin = new int[32];
            int index = 0;

            while (input1 > 0)
            {
                bin[index++] = input1 % 2;
                input1 /= 2;
            }

            for (int i = index - 1; i >= 0; i--)
                Console.Write(bin[i]);
        }
    }
}
