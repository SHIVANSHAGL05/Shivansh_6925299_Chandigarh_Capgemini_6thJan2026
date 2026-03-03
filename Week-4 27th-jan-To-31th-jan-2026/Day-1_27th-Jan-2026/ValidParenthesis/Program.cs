using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        Stack<char> stack = new Stack<char>();

        foreach (char c in s)
        {
            if (c == '(' || c == '{' || c == '[')
                stack.Push(c);
            else
            {
                if (stack.Count == 0) { Console.WriteLine("NO"); return; }

                char top = stack.Pop();
                if ((c == ')' && top != '(') ||
                    (c == '}' && top != '{') ||
                    (c == ']' && top != '['))
                {
                    Console.WriteLine("NO");
                    return;
                }
            }
        }

        Console.WriteLine(stack.Count == 0 ? "YES" : "NO");
    }
}
