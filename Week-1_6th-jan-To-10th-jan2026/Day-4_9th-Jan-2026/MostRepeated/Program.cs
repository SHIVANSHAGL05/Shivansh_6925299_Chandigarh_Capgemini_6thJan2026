namespace MostRepeated
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 2, 2, 2, 2, 3, 3, 3, 3, 4 };
            Dictionary<int, int> freq = new Dictionary<int, int>();

            foreach (int n in arr)
            {
                if (freq.ContainsKey(n))
                    freq[n]++;
                else
                    freq[n] = 1;
            }

            int max = 0;
            foreach (int v in freq.Values)
                if (v > max) max = v;

            foreach (var item in freq)
                if (item.Value == max)
                    Console.Write(item.Key + " ");
        }
    }
}
