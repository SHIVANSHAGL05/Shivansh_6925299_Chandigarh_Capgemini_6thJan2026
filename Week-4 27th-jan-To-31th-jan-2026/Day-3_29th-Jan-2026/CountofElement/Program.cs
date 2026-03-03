namespace CountofElement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int size = int.Parse(Console.ReadLine());
            string[] arr = new string[size];

            for (int i = 0; i < size; i++)
            {
                arr[i] = Console.ReadLine();
            }

            char ch = char.Parse(Console.ReadLine());

            int result = UserCode.GetCount(size, arr, ch);

            if (result == -1)
                Console.WriteLine("No elements Found");
            else if (result == -2)
                Console.WriteLine("Only alphabets should be given");
            else
                Console.WriteLine(result);
        }
    }
}
