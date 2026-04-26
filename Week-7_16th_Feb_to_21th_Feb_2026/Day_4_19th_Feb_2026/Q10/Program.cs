using System;

class Program
{
    static void Main()
    {
        int m = int.Parse(Console.ReadLine());

        if (m < 1 || m > 12)
            Console.WriteLine("Invalid Month Entered");
        else if (m >= 3 && m <= 5)
            Console.WriteLine("Spring");
        else if (m >= 6 && m <= 8)
            Console.WriteLine("Summer");
        else if (m >= 9 && m <= 11)
            Console.WriteLine("Autumn");
        else
            Console.WriteLine("Winter");
    }
}
