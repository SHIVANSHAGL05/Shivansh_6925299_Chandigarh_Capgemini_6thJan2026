using System.Text.RegularExpressions;

namespace ElectricityBillCalculation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter first meter reading : ");
            string input1 = Console.ReadLine();

            Console.WriteLine("Enter second meter reading : ");
            string input2 = Console.ReadLine();

            Console.WriteLine("Enter rate per unit: ");
            int rate = int.Parse(Console.ReadLine());

            string num1 = Regex.Match(input1, @"\d+").Value;
            string num2 = Regex.Match(input2, @"\d+").Value;

            long reading1 = long.Parse(num1);
            long reading2 = long.Parse(num2);

            long diff = Math.Abs(reading1 - reading2);
            long bill = diff * rate;

            Console.WriteLine("Electricity Bill Amount is: " + bill);
        }
    }
}

// testcase :: AAAAA12345, AAAAA23456, 4 ==> 44444