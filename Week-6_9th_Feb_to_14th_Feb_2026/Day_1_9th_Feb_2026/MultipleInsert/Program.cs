namespace MultipleInsert
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the main string:");
            string mainString = Console.ReadLine();

            Console.WriteLine("Enter number of substrings to insert:");
            int n = int.Parse(Console.ReadLine());

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine();
                Console.WriteLine("Enter substring " + (i + 1) + ":");
                string sub = Console.ReadLine();

                Console.WriteLine("Enter position to insert:");
                int pos = int.Parse(Console.ReadLine());

                if (pos < 0 || pos > mainString.Length)
                {
                    Console.WriteLine("Invalid position. Skipping insertion.");
                }
                else
                {
                    mainString = mainString.Insert(pos, sub);
                    Console.WriteLine("Substring inserted successfully.");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Final string after all insertions:");
            Console.WriteLine(mainString);
            Console.WriteLine();
        }
    }
}
