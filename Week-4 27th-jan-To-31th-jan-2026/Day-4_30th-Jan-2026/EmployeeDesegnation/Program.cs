using System;
using System.Collections.Generic;
using System.Globalization;

namespace EmployeeDesegnation
{
    internal class Program
    {
        static string[] getEmployee(string[] input1, string input2)
        {
            foreach (string s in input1)
            {
                if (!IsValid(s)) return new string[] { "Invalid Input" };
            }
            if (!IsValid(input2)) return new string[] { "Invalid Input" };

            List<string> result = new List<string>();
            int totalEmployees = input1.Length / 2;

            for (int i = 1; i < input1.Length; i += 2)
            {
                if (input1[i].Equals(input2, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(input1[i - 1]);
                }
            }

            if (result.Count == 0)
            {
                return new string[] { "No employee for " + input2 + " designation" };
            }

            if (result.Count == totalEmployees)
            {
                return new string[] { "All employees belong to same " + input2 + " designation" };
            }

            return result.ToArray();
        }

        static bool IsValid(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            foreach (char c in str)
            {
                if (!char.IsLetter(c) && c != ' ')
                    return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0 || n % 2 != 0)
            {
                return;
            }

            string[] arr = new string[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = Console.ReadLine();
            }

            string desig = Console.ReadLine();

            string[] output = getEmployee(arr, desig);

            foreach (string s in output)
            {
                Console.WriteLine(s);
            }
        }
    }
}