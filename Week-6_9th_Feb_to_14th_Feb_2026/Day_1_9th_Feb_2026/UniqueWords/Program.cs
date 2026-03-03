using System;
using System.Collections.Generic;
using System.Linq;
class Program{
    static void Main(){
        Console.WriteLine("Enter number of words:");
        int n = Convert.ToInt32(Console.ReadLine());

        List<string> words = new List<string>();
        Console.WriteLine("Enter words:");
        for (int i = 0; i < n; i++){
            words.Add(Console.ReadLine());
        }

        Dictionary<string, int> freq = new Dictionary<string, int>();

        foreach (string word in words) {
            string key = String.Concat(word.OrderBy(c => c));
            if (freq.ContainsKey(key))
                freq[key]++;
            else
                freq[key] = 1;
        }

        List<string> result = new List<string>();

        foreach (string word in words){
            string key = String.Concat(word.OrderBy(c => c));
            if (freq[key] == 1)
                result.Add(word);
        }

        Console.WriteLine("Unique words:");
        foreach (string w in result) {
            Console.WriteLine(w);
        }
    }
}