namespace E_Commerce
{
    class Product
    {
        protected string name;
        protected double price;

        public Product(string n, double p)
        {
            name = n; price = p;
        }

        public virtual void Display() => Console.WriteLine(name + " " + price);
    }

    class Electronics : Product
    {
        public Electronics(string n, double p) : base(n, p) { }
    }

    class Books : Product
    {
        public Books(string n, double p) : base(n, p) { }
    }

    class Program
    {
        static void Main()
        {
            Product p = new Electronics("Laptop", 55000);
            p.Display();
        }
    }
}
