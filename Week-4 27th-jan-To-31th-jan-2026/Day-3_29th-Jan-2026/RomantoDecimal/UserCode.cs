using System;
using System.Collections.Generic;
using System.Text;

namespace RomantoDecimal
{
    internal class UserCode
    {
        public static int convertRomanToDecimal(string roman)
        {
            Dictionary<char, int> map = new Dictionary<char, int>()
        {
            {'I',1},{'V',5},{'X',10},{'L',50},
            {'C',100},{'D',500},{'M',1000}
        };

            int sum = 0;

            foreach (char c in roman)
            {
                if (!map.ContainsKey(c))
                {
                    return -1;
                }
                sum += map[c];
            }
            return sum;
        }
    }
}
