using System;

class Program
{
    static void Main()
    {
        int limit = 20;

        for (int a = 1; a <= limit; a++)
            for (int b = 1; b <= limit; b++)
                for (int c = 1; c <= limit; c++)
                    if (a * a + b * b == c * c)
                        Console.WriteLine($"{a} {b} {c}");
    }
}
