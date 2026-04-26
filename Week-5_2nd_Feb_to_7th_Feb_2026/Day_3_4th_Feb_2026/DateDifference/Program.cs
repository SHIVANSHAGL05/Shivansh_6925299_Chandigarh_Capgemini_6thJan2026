namespace DateDifference
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter first date (dd/MM/yyyy):");
            string input1 = Console.ReadLine();

            Console.WriteLine("Enter second date (dd/MM/yyyy):");
            string input2 = Console.ReadLine();

            DateTime date1 = DateTime.ParseExact(input1, "dd/MM/yyyy", null);
            DateTime date2 = DateTime.ParseExact(input2, "dd/MM/yyyy", null);

            int days = Math.Abs((date2 - date1).Days);

            Console.WriteLine("Output: " + days + " days");
        }
    }
}
