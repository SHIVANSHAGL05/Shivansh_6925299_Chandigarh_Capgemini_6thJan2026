using System;
using System.Collections.Generic;
using System.Text;

namespace CountofElement
{
    internal class UserCode
    {
        public static int GetCount(int size, string[] arr, char ch)
        {
            int count = 0;
            ch = Char.ToLower(ch);

            foreach (string s in arr)
            {
                foreach (char c in s)
                {
                    if (!Char.IsLetter(c))
                    {
                        return -2;
                    }
                }

                if (Char.ToLower(s[0]) == ch)
                {
                    count++;
                }
            }

            if (count == 0)
                return -1;

            return count;
        }
    }
}
