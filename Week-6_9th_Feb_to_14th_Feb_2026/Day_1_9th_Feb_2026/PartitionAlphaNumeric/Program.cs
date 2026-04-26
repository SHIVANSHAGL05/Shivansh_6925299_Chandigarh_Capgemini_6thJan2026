namespace PartitionAlphaNumeric
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the string :: ");
            string st = Console.ReadLine();
            string num = "";
            string alpha = "";

            foreach(char ch in st)
            {
                if (char.IsDigit(ch))
                {
                    num += ch;
                }
                else if (char.IsLetter(ch))
                {
                    alpha += ch;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Left part (numbers): " + num);
            Console.WriteLine("Right part (alphabets): " + alpha);
            Console.ReadLine();

        }
    }
}
