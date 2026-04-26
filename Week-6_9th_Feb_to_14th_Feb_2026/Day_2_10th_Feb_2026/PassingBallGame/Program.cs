namespace PassingBallGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of friends:");
            int n = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter number of seconds:");
            int t = int.Parse(Console.ReadLine());

            int current = 0;
            int direction = 1;
            int from = 0, to = 0;

            for (int i = 1; i <= t; i++)
            {
                if (current == n - 1)
                    direction = -1;
                else if (current == 0)
                    direction = 1;

                from = current;
                to = current + direction;

                current = to;
            }

            Console.WriteLine("Last pass: Friend " + from + " passed to Friend " + to);
        }
    }
}


/*
 * n = 4, t = 10
 * output : Last pass: Friend 3 passed to Friend 2
 */