using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        Dictionary<char, int> freq = new Dictionary<char, int>();

        foreach (char c in s)
            freq[c] = freq.ContainsKey(c) ? freq[c] + 1 : 1;

        foreach (char c in s)
        {
            if (freq[c] == 1)
            {
                Console.WriteLine(c);
                return;
            }
        }

        Console.WriteLine("None");
    }
}
