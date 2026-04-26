using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    class Sale
    {
        public string Region { get; set; }
        public decimal Amount { get; set; }
    }

    static void Main()
    {
        List<Sale> sales = new List<Sale>
        {
            new Sale { Region = "North", Amount = 1200 },
            new Sale { Region = "South", Amount = 800 },
            new Sale { Region = "North", Amount = 500 },
            new Sale { Region = "East", Amount = 900 },
            new Sale { Region = "South", Amount = 700 }
        };

        var totals = sales
                        .GroupBy(s => s.Region)
                        .Select(g => new
                        {
                            Region = g.Key,
                            Total = g.Sum(x => x.Amount),
                            Count = g.Count()
                        });

        foreach (var t in totals)
        {
            Console.WriteLine($"{t.Region} : Total={t.Total}, Count={t.Count}");
        }
    }
}
