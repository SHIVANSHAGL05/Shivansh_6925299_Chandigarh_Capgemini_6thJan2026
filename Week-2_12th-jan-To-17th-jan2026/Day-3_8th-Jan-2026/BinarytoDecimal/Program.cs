namespace BinarytoDecimal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int input1 = Convert.ToInt32(Console.ReadLine());
            int output = 0;

            if (input1 > 11111)
            {
                Console.WriteLine("-2");
                return;
            }

            int temp = input1;
            int baseVal = 1;

            while (temp > 0)
            {
                int d = temp % 10;
                if (d != 0 && d != 1)
                {
                    Console.WriteLine("-1");
                    return;
                }
                output += d * baseVal;
                baseVal *= 2;
                temp /= 10;
            }

            Console.WriteLine(output);
        }
    }
}
