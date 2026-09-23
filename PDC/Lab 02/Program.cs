using System;
using System.Threading;

class Program
{
    static void Worker(object? arg)
    {
       int id = (int)arg!;

        int cpu = Thread.GetCurrentProcessorId();

        Console.WriteLine($"Thread {id}: running on logical CPU {cpu}");
    }

    static void Main()
    {
        // Detect the number of logical processors
        int numCores = Environment.ProcessorCount;

        Console.WriteLine($"Detected logical cores: {numCores}");

        // Create exactly one Thread object per logical core
        Thread[] threads = new Thread[numCores];

        // Create and start the threads
        for (int i = 0; i < numCores; i++)
        {
            int idx = i;

            threads[i] = new Thread(() => Worker(idx));
            threads[i].Start();
        }

        // Wait for every thread to finish
        for (int i = 0; i < numCores; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine($"All {numCores} threads completed.");
        Console.WriteLine($"Total threads created: {numCores}");
    }
}