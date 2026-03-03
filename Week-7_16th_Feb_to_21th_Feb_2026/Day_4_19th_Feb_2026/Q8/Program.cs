using System;

class Program
{
    static void Main()
    {
        string s = "Move#Hash#to#Front";
        string hash = "", rest = "";

        foreach (char c in s)
        {
            if (c == '#') hash += c;
            else rest += c;
        }

        Console.WriteLine(hash + rest);
    }
}
