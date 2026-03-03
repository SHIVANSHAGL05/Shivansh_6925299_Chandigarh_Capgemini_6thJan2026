namespace SumOfAllTheDigitsOfTheNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter a number :: ");
                int input = Convert.ToInt32(Console.ReadLine());

                int sum = 0;
                while (input != 0)
                {
                    sum += (input % 10);
                    input /= 10;
                }

                Console.WriteLine("Sum of all digits of the input value :: ");
                Console.WriteLine(sum);
                Console.WriteLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Invalid input!!! Value must have atmost 9 digits");
            }
        }
    }
}
