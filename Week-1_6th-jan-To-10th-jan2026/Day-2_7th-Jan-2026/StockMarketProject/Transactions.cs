using System;

namespace StockMarketProject
{
    struct Transaction
    {
        public int StockId;
        public TransactionType Type;
        public int Quantity;
        public DateTime Date;

        public Transaction(int id, TransactionType type, int qty)
        {
            StockId = id;
            Type = type;
            Quantity = qty;
            Date = DateTime.Now;
        }
    }
}
