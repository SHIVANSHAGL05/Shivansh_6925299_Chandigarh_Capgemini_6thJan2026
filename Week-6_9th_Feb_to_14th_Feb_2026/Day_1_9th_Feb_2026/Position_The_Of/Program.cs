namespace Position_The_Of
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of lines :: ");
            int n = int.Parse(Console.ReadLine());

            string[] Lines = new string[n];
            Console.WriteLine("Enter the Lines below :: ");
            for(int i = 0;i < n; i++)
            {
                Lines[i] = Console.ReadLine();
            }


            Dictionary<int, List<string>> dt = new Dictionary<int, List<string>>();

            int lineNumber = 1;
            foreach(string Line in Lines)
            {
                List<int> thePos = new List<int>();
                List<int> ofPos = new List<int>();

                int index = 0;
                while((index = Line.IndexOf("the ", index)) != -1)
                {
                    thePos.Add(index+1);
                    index += 3;
                }

                index = 0;
                while((index = Line.IndexOf("of ", index)) != -1)
                {
                    ofPos.Add(index+1);
                    index += 2;
                }

                string theIdx = "the position : " + (thePos.Count == 0 ? "not present" : string.Join(", ", thePos));
                string ofIdx = "of position : " + (ofPos.Count == 0 ? "not present" : string.Join(", ", ofPos));

                if (!dt.ContainsKey(lineNumber))
                {
                    dt[lineNumber] = new List<string>();
                }
                dt[lineNumber].Add(theIdx);
                dt[lineNumber].Add(ofIdx);
                lineNumber++;
            }



            Console.WriteLine("\n\nFinal the and of positions :: ");
            foreach(KeyValuePair<int,List<string>> d in dt)
            {
                Console.Write("\nLine " + d.Key + " :: ");
                foreach(var val in d.Value)
                {
                    Console.Write(val + "    ");
                }
            }
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}


/*
 * 25. The program should input N lines of a string. 
 * In each line find the position of "the" and "of" and print. If it is not found first "-1".
 */