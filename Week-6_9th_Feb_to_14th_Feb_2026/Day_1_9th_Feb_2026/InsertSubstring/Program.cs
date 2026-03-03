namespace InsertSubstring
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the main string:");
            string mainString = Console.ReadLine();

            Console.WriteLine("Enter the substring to insert:");
            string subString = Console.ReadLine();

            Console.WriteLine("Enter the position where substring should be inserted:");
            int position = int.Parse(Console.ReadLine());

            if (position < 0 || position > mainString.Length)
            {
                Console.WriteLine("Invalid position entered.");
            }
            else
            {
                string result = mainString.Insert(position, subString);

                Console.WriteLine("Final String after insertion:");
                Console.WriteLine(result);
            }

            Console.WriteLine();
        }
    }
}
