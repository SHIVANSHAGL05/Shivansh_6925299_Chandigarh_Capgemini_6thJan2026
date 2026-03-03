using System;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine().ToLower();
        int count = 0;

        foreach (char c in s)
        {
            if ("aeiou".Contains(c))
                count++;
        }

        Console.WriteLine(count);
    }
}
