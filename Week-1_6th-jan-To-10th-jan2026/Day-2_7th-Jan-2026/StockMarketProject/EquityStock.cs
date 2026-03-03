using System;

namespace StockMarketProject
{
    public delegate void StockNotification(string message);

    class EquityStock : Stock
    {
        public StockNotification Notify;   

        public EquityStock(int id, string name, double price, int quantity)
            : base(id, name, price, quantity)
        {
        }

        public override void Buy(int quantity)
        {
            SetStockQuantity(GetStockQuantity() + quantity);
            Notify?.Invoke(quantity + " stocks bought");
        }

        public override void Sell(int quantity)
        {
            if (quantity <= GetStockQuantity())
            {
                SetStockQuantity(GetStockQuantity() - quantity);
                Notify?.Invoke(quantity + " stocks sold");
            }
            else
            {
                Notify?.Invoke("Insufficient stock");
            }
        }
    }
}
