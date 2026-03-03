namespace MaximumDeletionsConsecutiveCharacterPair
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string:");
            string s = Console.ReadLine();

            int count = 0;

            for (int i = 0; i < s.Length - 1;)
            {
                if (s[i] == s[i + 1])
                {
                    count++;
                    i += 2;
                }
                else
                {
                    i++;
                }
            }

            Console.WriteLine("Maximum deletions possible: " + count);
        }
    }
}

