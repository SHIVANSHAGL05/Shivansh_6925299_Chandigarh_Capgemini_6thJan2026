using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        List<string> products = new List<string>
        {
            "Pen", "Pencil", "Notebook", "Marker", "Paper"
        };

        var result = products
                        .Where(p => p.StartsWith("P"))
                        .Select(p => p.ToUpper());

        foreach (var item in result)
        {
            Console.WriteLine(item);
        }
    }
}
