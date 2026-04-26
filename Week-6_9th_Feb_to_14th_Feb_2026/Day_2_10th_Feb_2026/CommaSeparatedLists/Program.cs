namespace CommaSeparatedLists
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the first list of integers (comma-separated):");
            string input1 = Console.ReadLine();

            Console.WriteLine("Enter the second list of integers (comma-separated):");
            string input2 = Console.ReadLine();

            int[] list1 = Array.ConvertAll(input1.Split(','), int.Parse);
            int[] list2 = Array.ConvertAll(input2.Split(','), int.Parse);

            Console.WriteLine("Final output : ");
            foreach (int n in list1)
            {
                int sum = 0;

                foreach (int x in list2)
                {
                    if (x == n)
                        sum += x;
                }

                Console.WriteLine(n + "-" + sum);
            }

            Console.ReadLine();
        }
    }
}

/*
5,3,4,6
5,4,2,1,4,3,3,2,5,3

OUTPUT:
5 - 10
3 - 9
4 - 8
6 - 0
*/