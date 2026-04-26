namespace NextGreaterDivisible
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of elements:");
            int n = int.Parse(Console.ReadLine());

            int[] arr = new int[n];

            Console.WriteLine("Enter the elements:");
            for (int i = 0; i < n; i++)
            {
                arr[i] = int.Parse(Console.ReadLine());
            }

            int count = 0;

            for (int i = 0; i < n; i++)
            {
                bool found = false;

                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] > arr[i] && arr[j] % arr[i] == 0)
                    {
                        count++;
                        found = true;
                        break;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Count of elements having next greater divisible element:");
            Console.WriteLine(count);
        }
    }
}
