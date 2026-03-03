namespace SecondLargest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 2, 3, 4, 1, 9 };

            if (arr.Length < 0)
            {
                Console.WriteLine("-2");
                return;
            }

            foreach (int n in arr)
            {
                if (n < 0)
                {
                    Console.WriteLine("-1");
                    return;
                }
            }

            Array.Sort(arr);
            Console.WriteLine(arr[arr.Length - 2]);
        }
    }
}
