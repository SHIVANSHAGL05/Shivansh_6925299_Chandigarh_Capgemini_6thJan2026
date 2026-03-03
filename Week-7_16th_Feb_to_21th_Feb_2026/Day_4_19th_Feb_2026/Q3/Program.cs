namespace Q3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter value for a:");
            int a = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter value for b:");
            int b = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter value for c:");
            int c = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine(a * 3 + a * 2 * b + 2 * a * 2 * b + 2 * a * b * 2 + a * b * 2 + b * 3);


        }
    }
}
