using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        List<int> scores = new List<int> { 78, 92, 85, 92, 67, 88, 95 };

        var top3 = scores
                    .OrderByDescending(s => s)
                    .Distinct()
                    .Take(3);

        foreach (var score in top3)
        {
            Console.WriteLine(score);
        }
    }
}
