namespace FtoC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int f = Convert.ToInt32(Console.ReadLine());
            int output;

            if (f < 0)
                output = -1;
            else
                output = (f - 32) * 5 / 9;

            Console.WriteLine(output);
        }
    }
}
