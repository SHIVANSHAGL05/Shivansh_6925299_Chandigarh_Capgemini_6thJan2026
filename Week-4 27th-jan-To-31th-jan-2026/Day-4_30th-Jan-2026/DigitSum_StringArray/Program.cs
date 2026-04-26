namespace DigitSum_StringArray
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            string[] arr = new string[n];

            for (int i = 0; i < n; i++)
                arr[i] = Console.ReadLine();

            int result = UserCode.sumdigit(arr);
            Console.WriteLine(result);
        }
    }
}
