namespace ReversePipeWords
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a piped string :: ");
            string input = Console.ReadLine();

            string[] st = input.Split('|');
            Array.Reverse(st);
            string result = string.Join("|", st);
            Console.WriteLine("Final Output :: ");
            Console.WriteLine(result);
        }
    }
}
