namespace StockMarketProject
{
    internal class Program
    {
        interface IStockDetails
        {
            void AddStock();
            void UpdatePrice(double price);
            void UpdateQuantity(int qty);
            double GetPrice();
            int GetQuantity();
            void Display();  
        }

        interface ITradingOperations
        {
            void Buy(int qty);
            void Sell(int qty);
            double StockValue();
            bool CheckStock(int qty);
            double Profit(double sellingPrice);
        }
        class Stock : IStockDetails, ITradingOperations
        {
            int id;
            string name;
            double price;
            int quantity;

            public Stock(int id, string name, double price, int quantity)
            {
                this.id = id;
                this.name = name;
                this.price = price;
                this.quantity = quantity;
            }

            public void AddStock() { }

            public void UpdatePrice(double price)
            {
                this.price = price;
            }

            public void UpdateQuantity(int qty)
            {
                quantity += qty;
            }

            public double GetPrice()
            {
                return price;
            }

            public int GetQuantity()
            {
                return quantity;
            }

            public void Display()
            {
                Console.WriteLine(id + " " + name + " " + price + " " + quantity);
            }

            public void Buy(int qty)
            {
                quantity += qty;
            }

            public void Sell(int qty)
            {
                if (qty <= quantity)
                    quantity -= qty;
            }

            public double StockValue()
            {
                return price * quantity;
            }

            public bool CheckStock(int qty)
            {
                return qty <= quantity;
            }

            public double Profit(double sellingPrice)
            {
                return sellingPrice - price;
            }
        }
        static void Main(string[] args)
        {
            IStockDetails s = new Stock(101, "TCS", 3500, 100);
            s.Display();

            ITradingOperations t = (ITradingOperations)s;
            t.Buy(50);

            Console.WriteLine(t.StockValue());
        }
    }
}
