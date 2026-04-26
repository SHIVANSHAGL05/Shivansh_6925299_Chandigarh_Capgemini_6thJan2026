using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter N (array size):");
        int N = Convert.ToInt32(Console.ReadLine());

        int[] arr = new int[N];
        Console.WriteLine("Enter array elements:");
        for (int i = 0; i < N; i++)
        {
            arr[i] = Convert.ToInt32(Console.ReadLine());
        }

        int count = 0;

        for (int i = 0; i < N - 1; i++)
        {
            int sum = arr[i] + arr[i + 1];
            if (sum % N == 0)
            {
                count++;
            }
        }

        Console.WriteLine("Number of valid couples:");
        Console.WriteLine(count);
    }
}