namespace GrossSalary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int basic = 8000;
            int days = 30;
            int output;

            if (basic < 0)
                output = -1;
            else if (basic > 10000)
                output = -2;
            else if (days > 31)
                output = -3;
            else
            {
                int da = basic * 75 / 100;
                int hra = basic * 50 / 100;
                output = basic + da + hra;
            }

            Console.WriteLine(output);
        }
    }
}
