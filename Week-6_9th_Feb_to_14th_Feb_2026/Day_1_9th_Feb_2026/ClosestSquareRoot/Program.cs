
namespace ClosestSquareRoot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number :: ");
            int input = int.Parse(Console.ReadLine());

            int n = (int)Math.Round(Math.Sqrt(input));
            Console.WriteLine("Closest square :: " + n*n);
            Console.WriteLine();
        }
    }
}
