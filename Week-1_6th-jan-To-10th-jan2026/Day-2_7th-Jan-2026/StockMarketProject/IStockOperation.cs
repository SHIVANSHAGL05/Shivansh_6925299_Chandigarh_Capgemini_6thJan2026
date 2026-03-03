using System;
using System.Collections.Generic;
using System.Text;

namespace StockMarketProject
{
    interface IStockOperations
    {
        void Buy(int quantity);
        void Sell(int quantity);
        void Display();
    }
}
