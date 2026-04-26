using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Queue<string> tickets = new Queue<string>();
        Stack<string> actions = new Stack<string>();


        tickets.Enqueue("Ticket 1");
        tickets.Enqueue("Ticket 2");
        tickets.Enqueue("Ticket 3");

        string currentTicket = tickets.Dequeue();
        Console.WriteLine("Processing: " + currentTicket);

        actions.Push("Opened ticket");
        actions.Push("Responded to customer");
        actions.Push("Closed ticket");

        Console.WriteLine("Undo action: " + actions.Pop());

        Console.WriteLine("\nRemaining tickets:");
        foreach (var t in tickets)
            Console.WriteLine(t);
    }
}
