using System;

namespace GreatestElement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] a = { 5, 9, 3 };
            int[] b = { 7, 2, 8 };
            int[] output = new int[a.Length];

            if (a.Length != b.Length)
            {
                output[0] = -2;
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] < 0 || b[i] < 0)
                    {
                        output[0] = -1;
                        break;
                    }
                    output[i] = (a[i] > b[i]) ? a[i] : b[i];
                }
            }

            for (int i = 0; i < output.Length; i++)
            {
                Console.Write(output[i] + " ");
            }
        }
    }
}
