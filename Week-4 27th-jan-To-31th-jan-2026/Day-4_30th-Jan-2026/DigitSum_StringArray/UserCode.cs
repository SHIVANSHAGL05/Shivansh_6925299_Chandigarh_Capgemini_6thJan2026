using System;
using System.Collections.Generic;
using System.Text;

namespace DigitSum_StringArray
{
    internal class UserCode
    {
        public static int sumdigit(string[] input1)
        {
            int sum = 0;
            foreach (string s in input1) {
                foreach (char c in s)
                {
                    if (!char.IsLetterOrDigit(c))
                    {
                        return -1;
                    }
                    if (char.IsDigit(c))
                        sum += (c - '0');
                    }
                }
                return sum;
            }
        }
    }
