namespace BinaryString
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter binary string:");
            string s = Console.ReadLine();

            List<string> blocks = new List<string>();

            for (int i = 0; i < s.Length; i += 2)
            {
                if (i + 1 < s.Length)
                    blocks.Add("" + s[i] + s[i + 1]);
                else
                    blocks.Add("" + s[i]);
            }

            blocks.Sort();

            string rearranged = string.Join("", blocks);

            int maxLen = LongestNonDecreasing(rearranged);

            Console.WriteLine("Maximum length: " + maxLen);
        }

        static int LongestNonDecreasing(string s)
        {
            int max = 1;
            int current = 1;

            for (int i = 1; i < s.Length; i++)
            {
                if (s[i] >= s[i - 1])
                    current++;
                else
                    current = 1;

                if (current > max)
                    max = current;
            }

            return max;
        }
    }
}


/*
 * 110100 ==> 000111===> 6
 */