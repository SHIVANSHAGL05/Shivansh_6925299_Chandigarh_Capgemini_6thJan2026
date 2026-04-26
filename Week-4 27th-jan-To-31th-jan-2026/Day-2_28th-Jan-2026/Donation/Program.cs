namespace Donation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            string[] arr = new string[n];

            for (int i = 0; i < n; i++)
                arr[i] = Console.ReadLine();

            int location = int.Parse(Console.ReadLine());

            int result = UserCode.getDonation(arr, location);
            Console.WriteLine(result);
        }
    }
}
