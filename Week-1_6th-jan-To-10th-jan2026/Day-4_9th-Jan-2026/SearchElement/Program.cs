namespace SearchElement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 5, 3, 9, 7 };
            int key = 9;
            int output = 1;

            if (arr.Length < 0)
                output = -2;
            else
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] < 0)
                    {
                        output = -1;
                        break;
                    }

                    if (arr[i] == key)
                    {
                        output = i;
                        break;
                    }
                }
            }

            Console.WriteLine(output);
        }
    }
}
