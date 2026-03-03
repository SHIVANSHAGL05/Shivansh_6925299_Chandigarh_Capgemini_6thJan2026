using System;

class Program
{
    static void Main()
    {
        int age = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine(age >= 18 ? "Eligible" : "Not Eligible");
    }
}
