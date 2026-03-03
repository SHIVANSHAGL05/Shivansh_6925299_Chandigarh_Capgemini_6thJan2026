using System;

namespace StockMarketProject
{
    abstract class Stock : IStockOperations
    {
        private int stockId;
        private string stockName;
        private double stockPrice;
        private int stockQuantity;

        protected Stock(int id, string name, double price, int quantity)
        {
            stockId = id;
            stockName = name;
            stockPrice = price;
            stockQuantity = quantity;
        }

        public int GetStockId() => stockId;
        public string GetStockName() => stockName;
        public double GetStockPrice() => stockPrice;
        public int GetStockQuantity() => stockQuantity;

        protected void SetStockPrice(double price) => stockPrice = price;
        protected void SetStockQuantity(int quantity) => stockQuantity = quantity;

        public abstract void Buy(int quantity);
        public abstract void Sell(int quantity);

        public void Display()
        {
            Console.WriteLine($"ID: {stockId}");
            Console.WriteLine($"Name: {stockName}");
            Console.WriteLine($"Price: {stockPrice}");
            Console.WriteLine($"Quantity: {stockQuantity}");
        }
    }
}
