using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;


namespace ProducerConsumerPipline
{
    internal class Program
    {
        static int processed = 0;

        static void Main()
        {
            RunSingle();
            RunBurst();
            RunShutdown();
            RunErrorIsolation();
        }

        static void RunSingle()
        {
            processed = 0;
            var bc = new BlockingCollection<string>(50);

            var producer = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                    bc.Add($"log-{i}");
                bc.CompleteAdding();
            });

            var consumer = Task.Run(() => Consume(bc));

            Task.WaitAll(producer, consumer);
            Console.WriteLine($"TC1 Processed={processed}");
            Console.WriteLine();
        }

        static void RunBurst()
        {
            processed = 0;
            var bc = new BlockingCollection<string>(50);

            Task[] producers = new Task[3];
            for (int p = 0; p < 3; p++)
            {
                int id = p;
                producers[p] = Task.Run(() =>
                {
                    for (int i = 0; i < 100; i++)
                        bc.Add($"P{id}-log-{i}");
                });
            }

            Task[] consumers = new Task[2];
            for (int c = 0; c < 2; c++)
                consumers[c] = Task.Run(() => Consume(bc));

            Task.WhenAll(producers).ContinueWith(_ => bc.CompleteAdding()).Wait();
            Task.WaitAll(consumers);

            Console.WriteLine($"TC2 Processed={processed}");
            Console.WriteLine();
        }

        static void RunShutdown()
        {
            processed = 0;
            var bc = new BlockingCollection<string>(10);

            var producer = Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                    bc.Add($"log-{i}");
                bc.CompleteAdding();
            });

            var consumer = Task.Run(() => Consume(bc));

            Task.WaitAll(producer, consumer);
            Console.WriteLine($"TC3 Processed={processed}");
            Console.WriteLine();
        }

        static void RunErrorIsolation()
        {
            processed = 0;
            var bc = new BlockingCollection<string>(20);

            var producer = Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    if (i % 10 == 0)
                        bc.Add("BAD");
                    else
                        bc.Add($"log-{i}");
                }
                bc.CompleteAdding();
            });

            Task[] consumers = new Task[3];
            for (int i = 0; i < 3; i++)
                consumers[i] = Task.Run(() => Consume(bc));

            Task.WaitAll(consumers);
            Console.WriteLine($"TC4 Processed={processed}");
            Console.WriteLine();
        }

        static void Consume(BlockingCollection<string> bc)
        {
            foreach (var item in bc.GetConsumingEnumerable())
            {
                try
                {
                    if (item == "BAD")
                        throw new Exception();
                    Interlocked.Increment(ref processed);
                }
                catch
                {
                }
            }
        }
    }

}




