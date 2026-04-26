using System;

class UserProgramCode
{
    public static string formString(string[] input1, int input2)
    {
        foreach (string s in input1)
            foreach (char c in s)
                if (!char.IsLetter(c))
                    return "-1";

        string result = "";

        foreach (string s in input1)
        {
            if (input2 - 1 < s.Length)
                result += s[input2 - 1];
            else
                result += "$";
        }
        return result;
    }
}
