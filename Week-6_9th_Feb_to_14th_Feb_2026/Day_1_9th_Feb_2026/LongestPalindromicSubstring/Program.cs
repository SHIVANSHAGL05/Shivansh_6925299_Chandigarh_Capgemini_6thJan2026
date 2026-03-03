namespace LongestPalindromicSubstring
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string :: ");
            string st = Console.ReadLine();

            int maxLen = 0;
            for(int i = 0;i < st.Length; i++)
            {
                int evenLen = Palindrome(st, i, i);
                int oddLen = Palindrome(st, i, i + 1);

                maxLen = Math.Max(maxLen, Math.Max(evenLen, oddLen));
            }

            Console.WriteLine("Length of Longest Palindromic substring :: " + maxLen);
            Console.ReadLine();
        }

        static int Palindrome(string st, int s, int e)
        {
            while(s >= 0 && e < st.Length && st[s] == st[e])
            {
                s--;
                e++;
            }
            return e - s - 1;
        }
    }
}

// testcase : babad ===> 3