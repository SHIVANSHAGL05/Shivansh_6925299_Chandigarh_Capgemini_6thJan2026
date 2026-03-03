namespace RemoveThenInsertSubstring
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string :: ");
            string st = Console.ReadLine();

            Console.WriteLine("Enter a substring to remove :: ");
            string substr = Console.ReadLine();

            Console.WriteLine("Enter the position for new sub-string :: ");
            int pos = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the new substring :: ");
            string newSubStr = Console.ReadLine();

            int idx = st.IndexOf(substr);
            if(idx == -1)
            {
                Console.WriteLine("Substring to remove not Exists!!");
                return;
            }
            else
            {
                st = st.Remove(idx, substr.Length);
                st = st.Insert(pos, newSubStr);
                Console.WriteLine("\nFinal String :: " + st);
            }
        }
    }
}
