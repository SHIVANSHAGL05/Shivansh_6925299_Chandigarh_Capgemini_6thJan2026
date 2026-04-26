namespace ListofElement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            List<int> list = new List<int>();

            for (int i = 0; i < n; i++)
                list.Add(int.Parse(Console.ReadLine()));

            int val = int.Parse(Console.ReadLine());

            List<int> res = UserProgramCode.GetElements(list, val);

            if (res.Count == 1 && res[0] == -1)
            {
                Console.WriteLine("No element found");
            }
            else
            {
                foreach (int x in res)
                    Console.Write(x + " ");
            }
        }
    }
    }