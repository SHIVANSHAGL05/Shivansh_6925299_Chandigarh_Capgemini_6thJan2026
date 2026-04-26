using System;

class Program
{
    static void Main()
    {
        string[] arr = { "23", "24.5" };

        foreach (string s in arr)
        {
            double n;
            if (!double.TryParse(s, out n))
            {
                Console.WriteLine("-1");
                return;
            }
        }
        Console.WriteLine("1");
    }
}
