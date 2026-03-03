using System;

class Program
{
    static void Main()
    {
        string s = "UDDDUDUU";
        int level = 0, valley = 0;

        foreach (char c in s)
        {
            if (c == 'U') level++;
            else level--;

            if (level == 0 && c == 'U')
                valley++;
        }
        Console.WriteLine(valley);
    }
}
