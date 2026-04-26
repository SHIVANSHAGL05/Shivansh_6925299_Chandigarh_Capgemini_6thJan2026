using System.Collections.Generic;
using System.Threading;


namespace OrderProcessingThread
{
    internal class Program
    {
        static int ProcessedCount = 0;
        static int Revenue = 0;
        static readonly object lockObj = new object();
        static Queue<int> orders = new Queue<int>();
        static bool useLock = true;

        static void Main(string[] args)
        {
            RunTest(workers: 1, totalOrders: 100, price: 100, lockFlag: true);
            RunTest(workers: 4, totalOrders: 1000, price: 50, lockFlag: true);
            RunTest(workers: 4, totalOrders: 1000, price: 50, lockFlag: false);
            RunTest(workers: 8, totalOrders: 10000, price: 1, lockFlag: true);
        }

        static void RunTest(int workers, int totalOrders, int price, bool lockFlag)
        {
            ProcessedCount = 0;
            Revenue = 0;
            useLock = lockFlag;
            orders.Clear();

            for (int i = 0; i < totalOrders; i++)
                orders.Enqueue(price);

            Thread[] threads = new Thread[workers];

            for (int i = 0; i < workers; i++)
            {
                threads[i] = new Thread(ProcessOrders);
                threads[i].Start();
            }

            foreach (var t in threads)
                t.Join();

            Console.WriteLine($"Workers={workers}, Orders={totalOrders}, Lock={useLock}");
            Console.WriteLine($"ProcessedCount={ProcessedCount}, Revenue={Revenue}");
            Console.WriteLine();
        }

        static void ProcessOrders()
        {
            while (true)
            {
                int price;
                lock (orders)
                {
                    if (orders.Count == 0)
                        return;
                    price = orders.Dequeue();
                }

                if (useLock)
                {
                    lock (lockObj)
                    {
                        ProcessedCount++;
                        Revenue += price;
                    }
                }
                else
                {
                    ProcessedCount++;
                    Revenue += price;
                }
            }
        }
    }

}


