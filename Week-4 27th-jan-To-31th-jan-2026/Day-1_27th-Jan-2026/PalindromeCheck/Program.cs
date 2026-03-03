using System;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        string rev = "";

        for (int i = s.Length - 1; i >= 0; i--)
            rev += s[i];

        if (s.Equals(rev))
            Console.WriteLine("Palindrome");
        else
            Console.WriteLine("Not Palindrome");
    }
}
