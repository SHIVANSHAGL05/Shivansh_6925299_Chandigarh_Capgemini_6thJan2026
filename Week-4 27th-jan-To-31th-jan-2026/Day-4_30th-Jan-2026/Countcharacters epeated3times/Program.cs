using System;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        int count = 0;

        for (int i = 0; i < s.Length - 2; i++)
        {
            if (s[i] == s[i + 1] && s[i + 1] == s[i + 2])
            {
                count++;
                while (i < s.Length - 1 && s[i] == s[i + 1])
                    i++;
            }
        }
        Console.WriteLine(count);
    }
}
