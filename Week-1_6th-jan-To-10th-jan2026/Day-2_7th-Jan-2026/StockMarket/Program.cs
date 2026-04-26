namespace StockMarket
{
            class StockMarket{
            public int stockId;
            public string stockName;
            public double stockPrice;
            public int stockQuantity;

            public StockMarket(int id, string name, double price, int quantity)
            {
                stockId = id;
                stockName = name;
                stockPrice = price;
                stockQuantity = quantity;
            }
            public void DisplayStock()
            {
                Console.WriteLine("Stock ID      : " + stockId);
                Console.WriteLine("Stock Name    : " + stockName);
                Console.WriteLine("Stock Price   : " + stockPrice);
                Console.WriteLine("Stock Quantity: " + stockQuantity);
            }
            public void BuyStock(int quantity)
            {
                stockQuantity += quantity;
                Console.WriteLine(quantity + " stocks bought successfully.");
            }

            public void SellStock(int quantity)
            {
                if (quantity <= stockQuantity)
                {
                    stockQuantity -= quantity;
                    Console.WriteLine(quantity + " stocks sold successfully.");
                }
                else
                {
                    Console.WriteLine("Not enough stock to sell!");
                }
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                StockMarket stock = new StockMarket(101, "TCS", 3500.50, 100);

            stock.DisplayStock();
            Console.WriteLine();
            stock.BuyStock(20);
            stock.DisplayStock();
            stock.SellStock(50);
            stock.DisplayStock();

            }
    }
}
