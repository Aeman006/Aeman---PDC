// ----TASK1----
// using System;
// using System.Diagnostics;

// class Program
// {
//     static void Main(string[] args)
//     {
//         if (args.Length > 0 && args[0] == "--child")
//         {
//             RunAsChild();
//         }
//         else
//         {
//             RunAsParent();
//         }
//     }

//     static void RunAsChild()
//     {
//         Console.WriteLine($"[Child] PID = {Environment.ProcessId}");

//         int counter = 100;
//         counter += 50;

//         Console.WriteLine($"[Child] final counter = {counter}");
//     }

//     static void RunAsParent()
//     {
//         Console.WriteLine($"[Parent] PID = {Environment.ProcessId}");

//         int counter = 100;
//         counter += 1;

//         // get the path of the current executable
//         string executable = Environment.ProcessPath!;

//         ProcessStartInfo startInfo = new ProcessStartInfo
//         {
//             FileName = executable,
//             UseShellExecute = false
//         };

//         // handle execution through the dotnet command
//         if (System.IO.Path.GetFileNameWithoutExtension(executable)
//             .Equals("dotnet", StringComparison.OrdinalIgnoreCase))
//         {
//             startInfo.ArgumentList.Add(
//                 System.Reflection.Assembly.GetEntryAssembly()!.Location);
//         }

//         // launch the child process
//         startInfo.ArgumentList.Add("--child");

//         Process child = Process.Start(startInfo)!;

//         // wait for the child to finish
//         child.WaitForExit();

//         Console.WriteLine($"[Parent] final counter = {counter}");

//         Console.WriteLine(
//             "[Parent] Parent and child counters were modified independently (separate address spaces).");
//     }
// }



//---TASK2---

// using System;
// using System.Threading;

// class Program
// {
//     static long[] data = new long[10_000_000];
//     static long[] partialSums;
//     static int numWorkers;

//     static void SumSlice(object? arg)
//     {
//         int idx = (int)arg!;

//         int sliceSize = data.Length / numWorkers;
//         int start = idx * sliceSize;
//         int end = (idx == numWorkers - 1)
//             ? data.Length
//             : start + sliceSize;

//         long sum = 0;

//         for (int i = start; i < end; i++)
//         {
//             sum += data[i];
//         }

//         partialSums[idx] = sum;
//     }

//     static void Main()
//     {
//         // fill the array with sequential values
//         for (int i = 0; i < data.Length; i++)
//         {
//             data[i] = i + 1;
//         }

//         // use the number of logical processors
//         numWorkers = Environment.ProcessorCount;
//         partialSums = new long[numWorkers];

//         Thread[] threads = new Thread[numWorkers];

//         // create and start worker threads
//         for (int i = 0; i < numWorkers; i++)
//         {
//             int idx = i;
//             threads[i] = new Thread(SumSlice);
//             threads[i].Start(idx);
//         }

//         // wait for all workers to finish
//         for (int i = 0; i < numWorkers; i++)
//         {
//             threads[i].Join();
//         }

//         // combine the partial sums
//         long threadedTotal = 0;

//         foreach (long partial in partialSums)
//         {
//             threadedTotal += partial;
//         }

//         // calculate the total sequentially
//         long sequentialTotal = 0;

//         foreach (long value in data)
//         {
//             sequentialTotal += value;
//         }

//         Console.WriteLine($"Array size: {data.Length}");
//         Console.WriteLine($"Worker threads: {numWorkers}");
//         Console.WriteLine($"Threaded total: {threadedTotal}");
//         Console.WriteLine($"Sequential total: {sequentialTotal}");
//         Console.WriteLine($"Match: {threadedTotal == sequentialTotal}");
//     }
// }


// ---TASK3---

// using System;
// using System.Diagnostics;
// using System.Threading;

// class Program
// {
//     const int Iterations = 50;

//     static void Main(string[] args)
//     {
//         // exit immediately when running as a child
//         if (args.Length > 0 && args[0] == "--child")
//         {
//             return;
//         }

//         string executable = Environment.ProcessPath!;

//         // measure process creation and waiting time
//         Stopwatch processStopwatch = Stopwatch.StartNew();

//         for (int i = 0; i < Iterations; i++)
//         {
//             ProcessStartInfo startInfo = new ProcessStartInfo
//             {
//                 FileName = executable,
//                 UseShellExecute = false
//             };

//             startInfo.ArgumentList.Add("--child");

//             Process child = Process.Start(startInfo)!;
//             child.WaitForExit();
//             child.Dispose();
//         }

//         processStopwatch.Stop();

//         // measure thread creation and joining time
//         Stopwatch threadStopwatch = Stopwatch.StartNew();

//         for (int i = 0; i < Iterations; i++)
//         {
//             Thread t = new Thread(() => { });

//             t.Start();
//             t.Join();
//         }

//         threadStopwatch.Stop();

//         // calculate average time per process and thread
//         double avgProcessMs =
//             processStopwatch.Elapsed.TotalMilliseconds / Iterations;

//         double avgThreadMs =
//             threadStopwatch.Elapsed.TotalMilliseconds / Iterations;

//         double ratio = avgProcessMs / avgThreadMs;

//         Console.WriteLine($"Iterations: {Iterations}");
//         Console.WriteLine(
//             $"Average process creation time: {avgProcessMs:F3} ms");

//         Console.WriteLine(
//             $"Average thread creation time: {avgThreadMs:F3} ms");

//         Console.WriteLine(
//             $"Process creation was {ratio:F1}x more expensive than thread creation.");
//     }
// }


--task4--

using System;
using System.Threading;

class Program
{
    static void Worker()
    {
        Thread.Sleep(200);
    }

    static void Main()
    {
        Thread t = new Thread(Worker);

        // conceptually: New
        Console.WriteLine($"After creation: {t.ThreadState}");

        t.Start();

        // conceptually: Runnable/Ready or Running
        Console.WriteLine($"Immediately after Start(): {t.ThreadState}");

        // give the worker time to enter its sleep
        Thread.Sleep(50);

        // conceptually: Blocked/Waiting
        Console.WriteLine($"While worker is sleeping: {t.ThreadState}");

        t.Join();

        // conceptually: Terminated
        Console.WriteLine($"After Join() completes: {t.ThreadState}");
    }
}