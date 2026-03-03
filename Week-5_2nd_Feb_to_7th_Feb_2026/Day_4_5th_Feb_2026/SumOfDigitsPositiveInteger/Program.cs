namespace SumOfDigitsPositiveInteger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a positive integer:");
            string input = Console.ReadLine();

            int sum = 0;

            foreach (char c in input)
                sum += c - '0';

            Console.WriteLine("Sum of digits: " + sum);
        }
    }
}
