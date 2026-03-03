using System;
using System.Threading;

namespace StockMarketProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Stock stock = new EquityStock(101, "TCS", 3500, 100);

            EquityStock eq = (EquityStock)stock;
            eq.Notify = msg => Console.WriteLine("NOTIFY: " + msg);

            stock.Display();
            stock.Buy(20);
            stock.Sell(10);

            Console.WriteLine("Total Value: " + stock.TotalValue());

            Thread t = new Thread(() =>
            {
                Thread.Sleep(2000);
                Console.WriteLine("Background thread executed");
            });
            t.Start();
        }
    }
}
