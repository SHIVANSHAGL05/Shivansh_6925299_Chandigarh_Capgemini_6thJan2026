using System;

class Program
{
    static void Main(string[] args)
    {
        string input = Console.ReadLine();
        string result = UserProgramCode.nextString(input);
        Console.WriteLine(result);
    }
}

class UserProgramCode
{
    public static string nextString(string input1)
    {
        foreach (char ch in input1)
        {
            if (!char.IsLetter(ch))
                return "Invalid input";
        }

        string vowels = "aeiou";
        string vowelsUpper = "AEIOU";
        string result = "";

        foreach (char ch in input1)
        {
            if (vowels.Contains(ch))
            {
                result += NextConsonant(ch);
            }
            else if (vowelsUpper.Contains(ch))
            {
                result += NextConsonant(ch);
            }
            else
            {
                result += NextVowel(ch);
            }
        }

        return result;
    }

    static char NextConsonant(char ch)
    {
        char c = ch;
        do
        {
            c++;
            if (c > 'z' && char.IsLower(ch)) c = 'a';
            if (c > 'Z' && char.IsUpper(ch)) c = 'A';
        }
        while ("aeiouAEIOU".Contains(c));

        return c;
    }

    static char NextVowel(char ch)
    {
        char c = ch;
        do
        {
            c++;
            if (c > 'z' && char.IsLower(ch)) c = 'a';
            if (c > 'Z' && char.IsUpper(ch)) c = 'A';
        }
        while (!"aeiouAEIOU".Contains(c));

        return c;
    }
}
