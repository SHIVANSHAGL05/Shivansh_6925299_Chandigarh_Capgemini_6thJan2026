using System;

class Program
{
    static void Main()
    {
        int[,] a = {
            {1,1,1,1},
            {2,2,2,2},
            {3,3,3,3},
            {4,4,4,4}
        };

        int[,] b = {
            {1,1,1,1},
            {2,2,2,2},
            {3,3,3,3},
            {4,4,4,4}
        };

        bool same = true;

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                if (a[i, j] != b[i, j])
                    same = false;

        Console.WriteLine(same ? "Matrices are identical" : "Not identical");
    }
}
