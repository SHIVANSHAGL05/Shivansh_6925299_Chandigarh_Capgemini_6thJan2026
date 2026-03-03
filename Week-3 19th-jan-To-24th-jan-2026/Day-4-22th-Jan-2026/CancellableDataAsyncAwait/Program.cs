using System;
using System.Threading;
using System.Threading.Tasks;

namespace CancellableDataAsyncAwait
{
    internal class Program
    {
        static async Task Main()
        {
            await RunNormal();
            await RunManualCancel();
            await RunTimeoutCancel();
        }

        static async Task RunNormal()
        {
            using var cts = new CancellationTokenSource();
            int imported = await ImportAsync(1000, cts.Token);
            Console.WriteLine($"TC1 Imported = {imported}");
            Console.WriteLine();
        }

        static async Task RunManualCancel()
        {
            using var cts = new CancellationTokenSource();
            var task = ImportAsync(1000, cts.Token);

            await Task.Delay(300);
            cts.Cancel();
            cts.Cancel();

            try
            {
                int imported = await task;
                Console.WriteLine($"TC2 Imported = {imported}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("TC2 Import canceled");
            }
            Console.WriteLine();
        }

        static async Task RunTimeoutCancel()
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(500);

            try
            {
                int imported = await ImportAsync(2000, cts.Token);
                Console.WriteLine($"TC3 Imported = {imported}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("TC3 Import canceled by timeout");
            }
            Console.WriteLine();
        }

        static async Task<int> ImportAsync(int totalRecords, CancellationToken token)
        {
            int imported = 0;

            for (int i = 1; i <= totalRecords; i++)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(10, token);
                imported++;

                if (imported % 100 == 0)
                    Console.WriteLine($"Progress: {imported}");
            }

            return imported;
        }
    }

}


