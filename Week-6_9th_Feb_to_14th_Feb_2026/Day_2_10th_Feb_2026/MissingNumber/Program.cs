namespace MissingNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter numbers (space separated):");
            int[] arr = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            int n = arr.Length + 1;

            int expectedSum = n * (n + 1) / 2;

            int actualSum = 0;
            foreach (int num in arr)
                actualSum += num;

            int missing = expectedSum - actualSum;

            Console.WriteLine("Missing number: " + missing);
        }
    }
}


/*
1 2 4 5

OUTPUT:
3
*/