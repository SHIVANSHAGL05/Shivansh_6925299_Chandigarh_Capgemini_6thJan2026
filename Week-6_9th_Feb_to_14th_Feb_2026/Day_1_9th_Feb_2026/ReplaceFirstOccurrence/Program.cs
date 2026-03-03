namespace ReplaceFirstOccurrence
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string:");
            string input = Console.ReadLine();

            Console.WriteLine("Enter the character to replace:");
            char oldChar = Convert.ToChar(Console.ReadLine());

            Console.WriteLine("Enter the character to replace with:");
            char newChar = Convert.ToChar(Console.ReadLine());

            int index = input.IndexOf(oldChar);

            if (index != -1)
            {
                input = input.Remove(index, 1).Insert(index, newChar.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("String after replacing '" + oldChar + "' with '" + newChar + "':");
            Console.WriteLine(input);
        }
    }
}
