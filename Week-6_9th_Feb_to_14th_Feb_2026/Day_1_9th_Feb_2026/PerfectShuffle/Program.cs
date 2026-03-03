namespace PerfectShuffle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter first string :: ");
            string first = Console.ReadLine();

            Console.WriteLine("Enter second string :: ");
            string second = Console.ReadLine();

            Console.WriteLine("Enter string you desire to check :: ");
            string third = Console.ReadLine();

            int i = 0, j = 0, k = 0;
            bool check = true;
            while(i < first.Length && j < second.Length && k < third.Length)
            {
                if (first[i] == third[k])
                {
                    i++;
                    k++;
                }
                else if (second[j] == third[k])
                {
                    j++;
                    k++;
                }
                else
                {
                    check = false;
                    break;
                }
            }

            while(i < first.Length && k < third.Length)
            {
                if(first[i] == third[k])
                {
                    i++;
                    k++;
                }
                else
                {
                    check = false;
                    break;
                }
            }

            while (j < second.Length && k < third.Length)
            {
                if (second[j] == third[k])
                {
                    j++;
                    k++;
                }
                else
                {
                    check = false;
                    break;
                }
            }

            Console.WriteLine(check == true ? "Pefectly Shuffled" : "Not Perfectly Shuffled");
            Console.ReadLine();
        }
    }
}

/*
 * x = "abc"
y = "def"
z = "adbcef"
 */