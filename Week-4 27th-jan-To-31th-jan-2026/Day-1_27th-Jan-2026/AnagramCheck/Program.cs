using System;

class Program
{
    static void Main()
    {
        string s1 = Console.ReadLine();
        string s2 = Console.ReadLine();

        if (s1.Length != s2.Length)
        {
            Console.WriteLine("Not Anagram");
            return;
        }

        char[] a1 = s1.ToCharArray();
        char[] a2 = s2.ToCharArray();

        Array.Sort(a1);
        Array.Sort(a2);

        Console.WriteLine(new string(a1) == new string(a2)
            ? "Anagram" : "Not Anagram");
    }
}
