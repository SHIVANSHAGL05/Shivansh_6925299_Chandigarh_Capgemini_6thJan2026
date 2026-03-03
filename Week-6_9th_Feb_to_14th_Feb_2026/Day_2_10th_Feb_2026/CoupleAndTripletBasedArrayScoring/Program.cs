namespace CoupleAndTripletBasedArrayScoring
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter array elements (space separated):");
            int[] arr = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            int score = 0;
            int n = arr.Length;

            for (int i = 0; i < n - 1; i++)
            {
                int sum = arr[i] + arr[i + 1];

                if (sum % 2 == 0)
                    score += 5;
            }

            for (int i = 0; i < n - 2; i++)
            {
                int sum = arr[i] + arr[i + 1] + arr[i + 2];
                int product = arr[i] * arr[i + 1] * arr[i + 2];

                if (sum % 2 != 0 && product % 2 == 0)
                    score += 10;
            }

            Console.WriteLine("Final score: " + score);
        }
    }
}

/*
1 2 3 4

OUTPUT:
Final score: 10
*/