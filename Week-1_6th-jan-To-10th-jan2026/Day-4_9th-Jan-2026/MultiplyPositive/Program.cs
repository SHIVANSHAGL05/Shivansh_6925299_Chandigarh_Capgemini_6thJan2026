namespace MultiplyPositive
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 1, 2, -3, 4 };
            int output1 = 1;

            if (arr.Length < 0)
                output1 = -2;
            else
            {
                foreach (int n in arr)
                    if (n > 0)
                        output1 *= n;
            }

            Console.WriteLine(output1);
        }
    }
}
