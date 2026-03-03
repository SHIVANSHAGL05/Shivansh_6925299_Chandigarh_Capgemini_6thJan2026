namespace Palindrome
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int num = 121;
            int temp = num, rev = 0;

            if (num < 0)
            {
                Console.WriteLine("-1");
                return;
            }

            while (temp > 0)
            {
                rev = rev * 10 + temp % 10;
                temp /= 10;
            }

            if (rev == num)
                Console.WriteLine("1");
            else
                Console.WriteLine("-2");
        }
    }
}
