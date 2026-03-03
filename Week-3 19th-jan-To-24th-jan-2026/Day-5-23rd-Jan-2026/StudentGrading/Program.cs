using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        Dictionary<int, int> grades = new Dictionary<int, int>
        {
            { 101, 85 },
            { 102, 40 },
            { 103, 72 },
            { 104, 35 }
        };

        // Average grade
        Func<int, bool> isAtRisk = g => g < 50;
        double average = grades.Values.Average();
        Console.WriteLine("Average Grade: " + average);

        // At-risk students
        Console.WriteLine("At-risk students:");
        foreach (var s in grades.Where(g => isAtRisk(g.Value)))
            Console.WriteLine($"Roll No: {s.Key}, Grade: {s.Value}");

        // Update grade
        grades[102] = 65;

        Console.WriteLine("\nAfter update:");
        foreach (var s in grades)
            Console.WriteLine($"Roll No: {s.Key}, Grade: {s.Value}");
    }
}
