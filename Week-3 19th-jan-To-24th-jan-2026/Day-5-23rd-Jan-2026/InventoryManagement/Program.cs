using System;
using System.Collections.Generic;
using System.Linq;

class Book
{
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

class Program
{
    static void Main()
    {
        List<Book> inventory = new List<Book>
        {
            new Book { Title = "C# Basics", Price = 500, Stock = 10 },
            new Book { Title = "LINQ Guide", Price = 300, Stock = 0 },
            new Book { Title = "ASP.NET Core", Price = 800, Stock = 5 }
        };

        // Find books cheaper than 600
        var cheapBooks = inventory.Where(b => b.Price < 600);
        Console.WriteLine("Books cheaper than 600:");
        foreach (var b in cheapBooks)
            Console.WriteLine(b.Title);

        // Increase price by 10%
        inventory.ForEach(b => b.Price += b.Price * 0.10m);

        // Remove out-of-stock books
        inventory.RemoveAll(b => b.Stock == 0);

        Console.WriteLine("\nUpdated Inventory:");
        foreach (var b in inventory)
            Console.WriteLine($"{b.Title} - {b.Price}");
    }
}
