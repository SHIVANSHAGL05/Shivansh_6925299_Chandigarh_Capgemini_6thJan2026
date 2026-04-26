using System.Text;

namespace MaximumDeletionsOfAnyConsecutiveCharacterPair
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string :: ");
            string st = Console.ReadLine();

            int count = 0;
            StringBuilder ans = new StringBuilder();
            for(int i = 0;i < st.Length; i++)
            {
                if(ans.Length == 0 || ans[ans.Length - 1] != st[i])
                {
                    ans.Append(st[i]);
                }
                else
                {
                    ++count;
                    ans.Remove(ans.Length - 1, 1);
                }
            }

            Console.WriteLine("Maximum Deletions of any Consecutive Character pairs :: ");
            Console.WriteLine(count);
            Console.WriteLine();
        }
    }
}
