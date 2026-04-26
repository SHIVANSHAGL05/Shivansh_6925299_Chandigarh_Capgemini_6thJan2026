using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    class CollageManagement
    {
        Dictionary<string, Dictionary<string, int>> studentRecords = new Dictionary<string, Dictionary<string, int>>();
        Dictionary<string, LinkedList<KeyValuePair<string, int>>> studentSubjectsOrder = new Dictionary<string, LinkedList<KeyValuePair<string, int>>>();

        Dictionary<string, Dictionary<string, int>> subjectsRecords = new Dictionary<string, Dictionary<string, int>>();
        Dictionary<string, LinkedList<KeyValuePair<string, int>>> subjectsStudentsOrder = new Dictionary<string, LinkedList<KeyValuePair<string, int>>>();

        public int AddStudent(string studentId, string subject, int marks)
        {
            if (!studentRecords.ContainsKey(studentId))
            {
                studentRecords[studentId] = new Dictionary<string, int>();
                studentSubjectsOrder[studentId] = new LinkedList<KeyValuePair<string, int>>();
            }

            if (!subjectsRecords.ContainsKey(subject))
            {
                subjectsRecords[subject] = new Dictionary<string, int>();
                subjectsStudentsOrder[subject] = new LinkedList<KeyValuePair<string, int>>();
            }

            if (!studentRecords[studentId].ContainsKey(subject))
            {
                studentRecords[studentId][subject] = marks;
                studentSubjectsOrder[studentId].AddLast(new KeyValuePair<string, int>(subject, marks));

                subjectsRecords[subject][studentId] = marks;
                subjectsStudentsOrder[subject].AddLast(new KeyValuePair<string, int>(studentId, marks));
            }
            else if (marks > studentRecords[studentId][subject])
            {
                studentRecords[studentId][subject] = marks;
                subjectsRecords[subject][studentId] = marks;

                var node = studentSubjectsOrder[studentId].First;
                while (node != null)
                {
                    if (node.Value.Key == subject)
                    {
                        node.Value = new KeyValuePair<string, int>(subject, marks);
                        break;
                    }
                    node = node.Next;
                }

                var node2 = subjectsStudentsOrder[subject].First;
                while (node2 != null)
                {
                    if (node2.Value.Key == studentId)
                    {
                        node2.Value = new KeyValuePair<string, int>(studentId, marks);
                        break;
                    }
                    node2 = node2.Next;
                }
            }

            return 1;
        }

        public int RemoveStudent(string studentId)
        {
            if (!studentRecords.ContainsKey(studentId))
                return 0;

            foreach (var subject in studentRecords[studentId].Keys)
            {
                subjectsRecords[subject].Remove(studentId);

                var node = subjectsStudentsOrder[subject].First;
                while (node != null)
                {
                    if (node.Value.Key == studentId)
                    {
                        subjectsStudentsOrder[subject].Remove(node);
                        break;
                    }
                    node = node.Next;
                }
            }

            studentRecords.Remove(studentId);
            studentSubjectsOrder.Remove(studentId);

            return 1;
        }

        public string TopStudent(string subject)
        {
            if (!subjectsStudentsOrder.ContainsKey(subject))
                return "";

            int max = subjectsRecords[subject].Values.Max();

            List<string> result = new List<string>();

            foreach (var entry in subjectsStudentsOrder[subject])
            {
                if (entry.Value == max)
                {
                    result.Add(entry.Key + " " + max);
                }
            }

            return string.Join("\n", result);
        }

        public string Result()
        {
            List<string> output = new List<string>();

            foreach (var student in studentRecords.Keys)
            {
                double avg = studentRecords[student].Values.Average();
                output.Add(student + " " + avg.ToString("F2"));
            }

            return string.Join("\n", output);
        }
    }

    public static void Main()
    {
        CollageManagement cm = new CollageManagement();

        cm.AddStudent("S1", "Math", 80);
        cm.AddStudent("S2", "Math", 90);
        cm.AddStudent("S3", "Math", 90);
        cm.AddStudent("S1", "Phy", 90);

        Console.WriteLine(cm.TopStudent("Math"));
        Console.WriteLine(cm.Result());

        cm.RemoveStudent("S1");
    }
}