namespace RemoveLastOccurrence
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string:");
            string str = Console.ReadLine();

            Console.WriteLine("Enter word to remove:");
            string word = Console.ReadLine();

            int index = str.LastIndexOf(word);

            if (index != -1)
            {
                str = str.Remove(index, word.Length);
            }

            Console.WriteLine("String after removing last occurrence:");
            Console.WriteLine(str);
        }
    }
}
