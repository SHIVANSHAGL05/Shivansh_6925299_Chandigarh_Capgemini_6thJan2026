using System;
using System.ComponentModel;

class Program
{
    static void Main()
    {
        int number = 0;
        bool valid = false;

        do
        {
            try
            {
                Console.Write("Enter a valid integer: ");
                number = Convert.ToInt32(Console.ReadLine());
                valid = true;
               
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input! Please enter numbers only.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Number is too large.");
            }
        }
        while (!valid);

        Console.WriteLine("You entered: " + number);
    }
}