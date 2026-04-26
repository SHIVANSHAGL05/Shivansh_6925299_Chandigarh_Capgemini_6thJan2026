namespace PalindromeScore
{
    internal class Program
    {
        static bool isPalindrome(string st)
        {
            int s = 0, e = st.Length - 1;
            while(s < e)
            {
                if (st[s] != st[e])
                {
                    return false;
                }
                s++; e--;
            }

            return true;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string :: ");
            string st = Console.ReadLine();

            int score = 0;
            for(int i = 0;i <= st.Length - 4; i++)
            {
                string checkString = st.Substring(i, 4);
                if(isPalindrome(checkString) == true)
                {
                    score += 5;
                }
            }

            for(int i = 0;i <= st.Length - 5; i++)
            {
                string checkString = st.Substring(i, 5);
                if(isPalindrome(checkString) == true)
                {
                    score += 10;
                }
            }

            Console.WriteLine("Final score :: " + score);
            Console.ReadLine();
        }
    }
}


/*
 * Example:  input string is ABCBAAAA  
 * score will be 15 because ABCBA - 10 points (palindrome of length 5) 
 * AAAA - 5 points (Palindrome of length 4)
 */