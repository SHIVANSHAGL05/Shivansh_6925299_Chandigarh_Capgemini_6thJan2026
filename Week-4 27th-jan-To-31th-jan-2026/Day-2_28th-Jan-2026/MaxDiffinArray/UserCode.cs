using System;
using System.Collections.Generic;
using System.Text;

namespace MaxDiffinArray
{
    internal class UserCode
    {
        public static int diffIntArray(int[] input1)
        {
            int n = input1.Length;

            if (n == 1 || n > 10)
                return -2;

            HashSet<int> set = new HashSet<int>();
            foreach (int x in input1)
            {
                if (x < 0)
                    return -1;
                if (!set.Add(x))
                    return -3;
            }

            int min = input1[0];
            int maxDiff = input1[1] - input1[0];

            for (int i = 1; i < n; i++)
            {
                maxDiff = Math.Max(maxDiff, input1[i] - min);
                min = Math.Min(min, input1[i]);
            }

            return maxDiff;
        }
    }
}
