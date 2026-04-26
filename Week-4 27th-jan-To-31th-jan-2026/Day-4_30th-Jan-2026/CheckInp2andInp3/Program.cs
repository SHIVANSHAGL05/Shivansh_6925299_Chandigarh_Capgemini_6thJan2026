using System;

class Program
{
    static void Main()
    {
        string input1 = "todayisc#exam";
        string input2 = "is";
        string input3 = "exam";

        int i2 = input1.IndexOf(input2);
        int i3 = input1.IndexOf(input3);

        Console.WriteLine(i2 != -1 && i3 != -1 && i3 > i2 ? 1 : 0);
    }
}
