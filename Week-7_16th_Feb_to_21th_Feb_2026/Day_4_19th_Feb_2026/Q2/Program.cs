using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int[] arr = { 1, 2, 3, 3, 4, 1, 4, 5, 1, 2 };
        Dictionary<int, int> map = new Dictionary<int, int>();

        foreach (int x in arr)
        {
            if (map.ContainsKey(x))
                map[x]++;
            else
                map[x] = 1;
        }

        foreach (var kv in map)
            Console.WriteLine($"{kv.Key} occurs {kv.Value} times");
    }
}
