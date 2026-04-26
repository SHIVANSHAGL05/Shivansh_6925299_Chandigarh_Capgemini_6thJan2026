using System;

class Program
{
    static void Main()
    {
        string dateStr = Console.ReadLine();
        DateTime date;

        if (!DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null,
            System.Globalization.DateTimeStyles.None, out date))
        {
            Console.WriteLine("-1");
            return;
        }

        Console.WriteLine(date.AddYears(1).DayOfWeek);
    }
}
