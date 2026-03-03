namespace ClosestIntegerHavingWholeNumberSquareRoot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a positive integer:");
            int n = int.Parse(Console.ReadLine());

            int root = (int)Math.Round(Math.Sqrt(n));
            int result = root * root;

            Console.WriteLine("Closest perfect square: " + result);
        }
    }
}
// testcases : 8 ==> 9 , 25 ==> 25, 18 ==> 16