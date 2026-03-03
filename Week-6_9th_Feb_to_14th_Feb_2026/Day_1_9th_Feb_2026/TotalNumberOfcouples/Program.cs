namespace TotalNumberOfcouples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the size of array :: ");
            int size = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the element :: ");
            int[] arr = new int[size];
            for(int i = 0;i < size; i++)
            {
                arr[i] = int.Parse(Console.ReadLine());
            }

            Console.WriteLine("Enter the divisor :: ");
            int n = int.Parse(Console.ReadLine());
            int count = 0;

            for(int i = 0;i < size; i++)
            {
                for(int j = i + 1;j < size; j++)
                {
                    if ((arr[i] + arr[j]) % n == 0)
                    {
                        ++count;
                    }
                }
            }

            Console.WriteLine("Total number of couples in arr :: " + count);
            Console.WriteLine();
        }
    }
}
