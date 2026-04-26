using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IsIsNot
{
     public class UserCode
    {
        public static string negativeString(string input)
        {
            string result = Regex.Replace(input, @"\bis\b", "is not");
            return result;
        }
    }
}
