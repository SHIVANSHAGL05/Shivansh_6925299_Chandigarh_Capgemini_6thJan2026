namespace Interface
{
    interface Inter1
    {
        void fi();
    }
    interface Inter2
    {
        void fi();
    }
    class c3 : Inter1, Inter2
    {
         void Inter1.fi()
        {
            Console.WriteLine("This overriding function");
        }
        void Inter2.fi()
        {
            Console.WriteLine("This is overriding");

        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            c3 obj1= new c3();
            Inter1 obj2= (Inter1)obj1;
            Inter2 obj3= (Inter2)obj1;
            obj2.fi();
            obj3.fi ();
        }
    }
}
