using System;
using System.Text;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string s = "hi this is my book";
        HashSet<char> seen = new HashSet<char>();
        StringBuilder sb = new StringBuilder();

        foreach (char c in s)
        {
            if (!seen.Contains(c))
            {
                seen.Add(c);
                sb.Append(c);
            }
        }
        Console.WriteLine(sb.ToString());
    }
}
