using System;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        int min = int.MaxValue;
        string result = "";

        for (int i = 0; i < n; i++)
        {
            string[] s = Console.ReadLine().Split(',');
            int price = int.Parse(s[1]);
            int discount = int.Parse(s[2]);

            int amount = price * discount / 100;

            if (amount < min)
            {
                min = amount;
                result = s[0];
            }
        }
        Console.WriteLine(result);
    }
}

