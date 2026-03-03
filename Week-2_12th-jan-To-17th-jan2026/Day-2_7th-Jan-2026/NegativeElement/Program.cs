namespace NegativeElement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 10, -5, 3, -1, 8 };

            if (arr.Length < 0)
            {
                Console.WriteLine("-1");
                return;
            }

            int count = 0;
            foreach (int n in arr)
                if (n >= 0) count++;

            int[] output = new int[count];
            int index = 0;

            foreach (int n in arr)
                if (n >= 0) output[index++] = n;

            Array.Sort(output);

            foreach (int n in output)
                Console.Write(n + " ");
        }
    }
}
