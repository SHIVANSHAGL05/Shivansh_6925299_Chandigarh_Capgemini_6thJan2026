namespace SearchingElement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 4, 7, 9, 2 };
            int search = 7;
            int output = -3;

            if (arr.Length < 0)
            {
                output = -2;
            }
            else
            {
                foreach (int n in arr)
                {
                    if (n < 0)
                    {
                        output = -1;
                        Console.WriteLine(output);
                        return;
                    }
                }

                foreach (int n in arr)
                {
                    if (n == search)
                    {
                        output = 1;
                        break;
                    }
                }
            }

            Console.WriteLine(output);
        }
    }
}
