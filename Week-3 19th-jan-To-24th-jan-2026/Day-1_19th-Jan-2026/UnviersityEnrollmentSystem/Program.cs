namespace UnviersityEnrollmentSystem
{
    class Person
    {
        protected string name;
        public Person(string name) { this.name = name; }
        public virtual void Display() => Console.WriteLine(name);
    }

    class Student : Person
    {
        public Student(string n) : base(n) { }
        public override void Display() => Console.WriteLine("Student: " + name);
    }

    class Professor : Person
    {
        public Professor(string n) : base(n) { }
        public override void Display() => Console.WriteLine("Professor: " + name);
    }

    class Program
    {
        static void Main()
        {
            Person p1 = new Student("Ravi");
            Person p2 = new Professor("Dr. Kumar");
            p1.Display();
            p2.Display();
        }
    }
}
