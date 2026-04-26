namespace CalculatorApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var calc = new Calculator();
            Console.WriteLine($"Add:{ calc.Add(5,3)}");
            Console.WriteLine($"Substract:{calc.Substract(5, 3)}");
            Console.WriteLine($"Multiply:{calc.Multiply(5, 3)}");
            Console.WriteLine($"Divide:{calc.Divide(10,2)}");
        }
    }
}
