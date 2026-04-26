namespace LeapYear
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int year = 2024;

            if (year < 0)
                Console.WriteLine("-1");
            else if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
                Console.WriteLine("Leap Year");
            else
                Console.WriteLine("Not Leap Year");
        }
    }
}
