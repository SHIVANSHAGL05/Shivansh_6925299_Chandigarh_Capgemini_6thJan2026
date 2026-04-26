namespace CountNonMultiplesInArray
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter array elements (space separated):");
            int[] arr = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            int count = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                bool divisible = false;

                for (int j = 0; j < arr.Length; j++)
                {
                    if (i != j && arr[i] % arr[j] == 0)
                    {
                        divisible = true;
                        break;
                    }
                }

                if (!divisible)
                    count++;
            }

            Console.WriteLine("Output: " + count);
        }
    }
}


/*
24 11 8 3 16

OUTPUT:
3
*/