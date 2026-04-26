using System;

namespace Lucky_Number
{
    class UserMainCode
    {
        public static int LuckyNumbers(int m, int n)
        {
            int count = 0;

            for (int i = m; i <= n; i++)
            {
                if (!IsPrime(i))
                {
                    if (SumDigits(i * i) == SumDigits(i) * SumDigits(i))
                        count++;
                }
            }
            return count;
        }

        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            for (int i = 2; i <= Math.Sqrt(n); i++)
                if (n % i == 0) return false;
            return true;
        }

        static int SumDigits(int n)
        {
            int sum = 0;
            while (n > 0)
            {
                sum += n % 10;
                n /= 10;
            }
            return sum;
        }
    }
}
