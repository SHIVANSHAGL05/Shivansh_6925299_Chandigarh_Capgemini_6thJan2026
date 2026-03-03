namespace VehicleRentalSystem
{
    class Vehicle
    {
        protected double rate;
        public Vehicle(double rate) { this.rate = rate; }
        public virtual double CalculateRent(int days) => rate * days;
    }

    class Car : Vehicle
    {
        public Car() : base(1000) { }
    }

    class Bike : Vehicle
    {
        public Bike() : base(300) { }
    }

    class Program
    {
        static void Main()
        {
            Vehicle v = new Car();
            Console.WriteLine(v.CalculateRent(3));
        }
    }
}
