namespace InsertSingleCharInString
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string :: ");
            string input = Console.ReadLine();
            Console.WriteLine("Enter the inserting character :: ");
            string ch = Console.ReadLine();
            Console.WriteLine("Enter the index :: ");
            int index = int.Parse(Console.ReadLine());

            input.Insert(index, ch);
            Console.WriteLine("Final Output :: " + input);
            Console.WriteLine();
        }
    }
}
