namespace SortAsceDsce
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 9, 3, 1, 6, 8, 2 };

            if (arr.Length < 0)
            {
                Console.WriteLine("-1");
                return;
            }

            int mid = arr.Length / 2;

            int[] firstHalf = new int[mid];
            int[] secondHalf = new int[arr.Length - mid];

            Array.Copy(arr, 0, firstHalf, 0, mid);
            Array.Copy(arr, mid, secondHalf, 0, arr.Length - mid);

            Array.Sort(firstHalf);
            Array.Sort(secondHalf);
            Array.Reverse(secondHalf);

            foreach (int n in firstHalf)
                Console.Write(n + " ");
            foreach (int n in secondHalf)
                Console.Write(n + " ");
        }
    }
}
