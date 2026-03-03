using System;
using System.Collections.Generic;
using System.Text;

namespace Donation
{
    internal class UserCode
    {
        public static int getDonation(string[] input1, int input2)
        {
            HashSet<string> set = new HashSet<string>();
            int sum = 0;

            foreach (string s in input1)
            {
                if (!set.Add(s))
                    return -1;

                foreach (char c in s)
                {
                    if (!Char.IsLetterOrDigit(c))
                        return -2;
                }

                string location = s.Substring(3, 3);
                int donation = int.Parse(s.Substring(6, 3));

                if (location == input2.ToString())
                {
                    sum += donation;
                }
            }
            return sum;
        }
    }
}
