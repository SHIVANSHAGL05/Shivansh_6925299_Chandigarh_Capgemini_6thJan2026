namespace DetectKey
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key:");
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.KeyChar >= '0' && keyInfo.KeyChar <= '9')
                Console.WriteLine("\nNumber pressed: " + keyInfo.KeyChar);
            else
                Console.WriteLine("\nNot allowed");
        }
    }
}
