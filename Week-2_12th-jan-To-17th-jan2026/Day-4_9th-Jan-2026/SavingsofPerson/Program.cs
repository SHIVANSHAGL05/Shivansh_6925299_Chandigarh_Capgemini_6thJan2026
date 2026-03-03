namespace SavingsofPerson
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int salary = 7000;
            int days = 30;
            int output;

            if (salary > 9000)
                output = -1;
            else if (salary < 0)
                output = -2;
            else if (days < 0)
                output = -4;
            else
            {
                int extra = (days == 31) ? 500 : 0;
                int food = salary * 50 / 100;
                int travel = salary * 20 / 100;
                output = salary + extra - (food + travel);
            }

            Console.WriteLine(output);
        }
    }
}
