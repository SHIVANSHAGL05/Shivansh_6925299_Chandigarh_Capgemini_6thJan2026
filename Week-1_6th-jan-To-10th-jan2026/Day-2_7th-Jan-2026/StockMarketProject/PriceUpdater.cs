using System;
using System.Threading;

namespace StockMarketProject
{
    class PriceUpdater
    {
        public static void UpdatePrice()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Price updated in background thread");
        }
    }
}
