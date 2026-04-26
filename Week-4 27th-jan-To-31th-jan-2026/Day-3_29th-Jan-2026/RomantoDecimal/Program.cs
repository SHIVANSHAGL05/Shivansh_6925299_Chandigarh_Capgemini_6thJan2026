namespace RomantoDecimal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            int result = UserCode.convertRomanToDecimal(input);
            Console.WriteLine(result);
        }
    }
}
