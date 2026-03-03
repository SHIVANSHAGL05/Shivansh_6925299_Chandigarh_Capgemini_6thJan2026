namespace Multipleofthree
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 3, 6, 1, 9 };
            int output = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < 0)
                {
                    output = -1;
                    break;
                }
                if (arr[i] % 3 == 0)
                    output++;
            }

            Console.WriteLine(output);
        }
    }
}
