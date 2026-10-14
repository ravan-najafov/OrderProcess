using System;
using System.Threading;

const int threadCount = 5;
const int ordersPerThread = 100;

int processedOrders = 0;

var semaphore = new SemaphoreSlim(2, 2);
var allProcessedEvent = new ManualResetEvent(false);

var waiterThread = new Thread(() =>
{
    allProcessedEvent.WaitOne();
    Console.WriteLine("Vse zakazi obrabotani");
});
waiterThread.Start();

var workers = new Thread[threadCount];
for (int i = 0; i < threadCount; i++)
{
    int workerId = i + 1;
    workers[i] = new Thread(() =>
    {
        for (int j = 0; j < ordersPerThread; j++)
        {
            semaphore.Wait();
            try
            {
                Thread.Sleep(2);
                Interlocked.Increment(ref processedOrders);
            }
            finally
            {
                semaphore.Release();
            }
        }
        Console.WriteLine($"{workerId} Patok: zakoncil 100 zakaz");
        
    });
    workers[i].Start();
}

foreach (var worker in workers)
{
    worker.Join();
}

Console.WriteLine("===============================");
Console.WriteLine($"Kolicestva obrabotanix zakazov: {processedOrders}");

allProcessedEvent.Set();
waiterThread.Join();

allProcessedEvent.Reset(); // ManualReseetEvent delayet snova false tak kak on ostayotsya true posle Set()