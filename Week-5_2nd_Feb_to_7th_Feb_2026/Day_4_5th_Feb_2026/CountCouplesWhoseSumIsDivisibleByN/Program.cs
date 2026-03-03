namespace CountCouplesWhoseSumIsDivisibleByN
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter array size (N):");
            int N = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter array elements separated by comma:");
            int[] arr = new int[N];

            for (int i = 0; i < N; i++)
            {
                arr[i] = int.Parse(Console.ReadLine());
            }

            int count = 0;

            for (int i = 0; i < N - 1; i++)
            {
                if ((arr[i] + arr[i + 1]) % N == 0)
                    count++;
            }

            Console.WriteLine("Number of valid couples: " + count);
        }
    }
}


/*
 * 
 * N = 4  
arr = [2,2,4,0]
Couples & Sums:

(2,2) → 4 ✔

(2,4) → 6 ✘

(4,0) → 4 ✔

Output:

Number of valid couples: 2

 * 
 */