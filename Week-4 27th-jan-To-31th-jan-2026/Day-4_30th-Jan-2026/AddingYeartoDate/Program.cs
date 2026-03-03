using System;

class Program
{
    static void Main()
    {
        string dateStr = Console.ReadLine();
        int years = Convert.ToInt32(Console.ReadLine());

        DateTime date;
        if (!DateTime.TryParse(dateStr, out date))
        {
            Console.WriteLine("-1");
            return;
        }
        if (years < 0)
        {
            Console.WriteLine("-2");
            return;
        }

        Console.WriteLine(date.AddYears(years).ToString("dd/MM/yyyy"));
    }
}
