namespace MultipleChoice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("What is the correct way to declare a variable to store an integer value in C#?");
            Console.WriteLine("a. int 1x=10;");
            Console.WriteLine("b. int x=10;");
            Console.WriteLine("c. float x=10.0f;");
            Console.WriteLine("d. string x=\"10\";");

            Console.Write("Choose the answer letter: ");
            char choice = Convert.ToChar(Console.ReadLine().ToLower());

            if (choice == 'b')
                Console.WriteLine("Correct choice!");
            else
                Console.WriteLine("Incorrect choice!");
        }
    }
}
