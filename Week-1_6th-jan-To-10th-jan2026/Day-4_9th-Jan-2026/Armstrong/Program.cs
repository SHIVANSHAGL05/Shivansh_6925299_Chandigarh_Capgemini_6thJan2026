namespace Armstrong{
    internal class Program {
        static void Main(string[] args) {
            int number = Convert.ToInt32(Console.ReadLine());
            int output1;
            if (number < 0)
                output1 = -1;
            else if (number > 999)
                output1 = -2;
            else
            {
                int sum = 0, temp = number;
                while (temp > 0)
                {
                    int d = temp % 10;
                    sum += d * d * d;
                    temp /= 10;
                }

                output1 = (sum == number) ? 1 : 0;
            }

            Console.WriteLine(output1);
        }
    }
}
