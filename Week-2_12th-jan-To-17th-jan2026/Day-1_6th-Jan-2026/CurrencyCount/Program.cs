namespace CurrencyCount
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int amount = Convert.ToInt32(Console.ReadLine());
            int output;

            if (amount < 0)
            {
                output = -1;
            }
            else
            {
                int count = 0;

                count += amount / 500;
                amount %= 500;

                count += amount / 100;
                amount %= 100;

                count += amount / 50;
                amount %= 50;

                count += amount / 10;
                amount %= 10;

                count += amount;

                output = count;
            }

            Console.WriteLine(output);
        }
    }
}
.