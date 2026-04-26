namespace Greatest_number
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter first number: ");
            int a = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter second number: ");
            int b = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter third number: ");
            int c = Convert.ToInt32(Console.ReadLine());

            int greatest;

            if (a >= b && a >= c)
                greatest = a;
            else if (b >= a && b >= c)
                greatest = b;
            else
                greatest = c;

            Console.WriteLine("Greatest number is: " + greatest);
        }
    }
}
