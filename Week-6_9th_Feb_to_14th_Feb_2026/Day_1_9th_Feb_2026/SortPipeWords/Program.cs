namespace SortPipeWords
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the pipe separated string :: ");
            string st = Console.ReadLine();

            string[] separted = st.Split('|');
            Array.Sort(separted);
            st = string.Join('|', separted);
            Console.WriteLine("Final string :: " + st);
        }
    }
}
