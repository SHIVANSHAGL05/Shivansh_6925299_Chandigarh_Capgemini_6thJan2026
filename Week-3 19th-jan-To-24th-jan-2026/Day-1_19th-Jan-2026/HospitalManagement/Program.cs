namespace HospitalManagement
{
    class Person
    {
        protected string name;
        public Person(string n) { name = n; }
        public virtual void Show() => Console.WriteLine(name);
    }

    class Doctor : Person
    {
        public Doctor(string n) : base(n) { }
        public override void Show() => Console.WriteLine("Doctor: " + name);
    }

    class Patient : Person
    {
        public Patient(string n) : base(n) { }
        public override void Show() => Console.WriteLine("Patient: " + name);
    }

    class Program
    {
        static void Main()
        {
            Person p = new Doctor("Dr. Sharma");
            p.Show();
        }
    }
}
