using System.Text;

namespace Q1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string s = "aabbbbeeeeffggg";
            int count = 1;

            for (int i = 0; i < s.Length; i++)
            {
                if (i + 1 < s.Length && s[i] == s[i + 1])
                    count++;
                else
                {
                    Console.Write(s[i] + count.ToString());
                    count = 1;
                }
            }

            Console.WriteLine(count.ToString());    
        }
    }
}
