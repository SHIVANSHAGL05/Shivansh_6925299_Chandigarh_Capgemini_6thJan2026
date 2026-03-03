namespace SortandInsert
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 2, 4, 6, 8 };
            int insert = 5;

            foreach (int n in arr)
            {
                if (n < 0)
                {
                    Console.WriteLine("-1");
                    return;
                }
            }

            Array.Sort(arr);
            int[] result = new int[arr.Length + 1];
            int i = 0, j = 0;

            while (i < arr.Length && arr[i] < insert)
                result[j++] = arr[i++];

            result[j++] = insert;

            while (i < arr.Length)
                result[j++] = arr[i++];

            foreach (int n in result)
                Console.Write(n + " ");
        }
    }
}
