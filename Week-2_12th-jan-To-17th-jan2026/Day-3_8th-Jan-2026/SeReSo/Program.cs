namespace SeReSo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] input1 = { 54, 26, 78, 32, 55 };
            int input2 = 5;
            int input3 = 78;

            if (input2 < 0)
            {
                Console.WriteLine("-2");
                return;
            }

            foreach (int n in input1)
            {
                if (n < 0)
                {
                    Console.WriteLine("-1");
                    return;
                }
            }

            bool found = false;
            List<int> list = new List<int>();

            foreach (int n in input1)
            {
                if (n == input3)
                    found = true;
                else
                    list.Add(n);
            }

            if (!found)
            {
                Console.WriteLine("-3");
                return;
            }

            list.Sort();

            foreach (int n in list)
                Console.Write(n + " ");
        }
    }
}
