using System;

namespace Delegate
{
    public delegate void Math(int x, int y);

    class MultiClass
    {
        public void Add(int x, int y)
        {
            Console.WriteLine("Add: " + (x + y));
        }

        public void Sub(int x, int y)
        {
            Console.WriteLine("Sub: " + (x - y));
        }

        public void Mul(int x, int y)
        {
            Console.WriteLine("Multiplication: " + (x * y));
        }

        public void Div(int x, int y)
        {
            if (y != 0)
                Console.WriteLine("Division: " + (x / y));
            else
                Console.WriteLine("Division by zero not allowed");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MultiClass obj = new MultiClass();

            Math m = null;

            m += obj.Add;
            m += obj.Sub;
            m += obj.Mul;
            m += obj.Div;

            m(10, 5);

        }
    }
}
