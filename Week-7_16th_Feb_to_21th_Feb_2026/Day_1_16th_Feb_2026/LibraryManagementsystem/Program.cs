using System;

class Program
{
    static void Main(string[] args)
    {
        var library = new Library();
        library.AddBook("C# Basics", 3);
        library.BorrowBook("C# Basics");
        Console.WriteLine($"Remaining copies: {library.GetBookCount("C# Basics")}");
    }
}
