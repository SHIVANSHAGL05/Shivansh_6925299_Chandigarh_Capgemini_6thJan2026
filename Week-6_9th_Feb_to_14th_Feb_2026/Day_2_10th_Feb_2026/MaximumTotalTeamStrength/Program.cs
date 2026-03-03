namespace MaximumTotalTeamStrength
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter employee skills (space separated):");
            int[] skills = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            Console.WriteLine("Enter team sizes (space separated):");
            int[] teams = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            Array.Sort(skills);
            Array.Sort(teams);

            int left = 0;
            int right = skills.Length - 1;
            int totalStrength = 0;

            for (int i = teams.Length - 1; i >= 0; i--)
            {
                int size = teams[i];
                int min = skills[left];
                int max = skills[right];

                totalStrength += min + max;

                left += size - 1;
                right--;
            }

            Console.WriteLine("Maximum total strength: " + totalStrength);
        }
    }
}


/*
 * skills: 1 3 5 7 9
teams: 2 3

output : 16
 */