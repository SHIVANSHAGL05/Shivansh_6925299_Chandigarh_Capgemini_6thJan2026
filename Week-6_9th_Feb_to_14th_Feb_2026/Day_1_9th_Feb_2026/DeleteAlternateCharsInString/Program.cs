namespace DeleteAlternateCharsInString
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string :: ");
            string st = Console.ReadLine();

            for(int i = 0; i < st.Length; i++)
            {
                st = st.Remove(i, 1);
            }

            Console.WriteLine("Final Output :: " + st);
            Console.WriteLine();
        }
    }
}
