namespace CountTriplets
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter array elements (space separated):");
            int[] arr = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            Console.WriteLine("Enter d:");
            int d = int.Parse(Console.ReadLine());

            int count = 0;
            int n = arr.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    for (int k = j + 1; k < n; k++)
                    {
                        if ((arr[i] + arr[j] + arr[k]) % d == 0)
                            count++;
                    }
                }
            }

            Console.WriteLine("Triplets count: " + count);
        }
    }
}

/*
 * 
 * 1 2 3 4 5
 *  3
 *  output ==> 4
 */
