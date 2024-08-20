// Filename: CheckCPU.cs
// Author: fang
// Created: August 20, 2024
// Description: This class represents measure cpu_usage from one of online example
//              https://medium.com/@jackwild/getting-cpu-usage-in-net-core-7ef825831b8b.

using System.Diagnostics;

namespace SwaggerDemo.Helper
{
    public class CheckCPU
    {
        public static async Task InvokeCPU()
        {
            var task = Task.Run(() => ConsumeCPU(50));

            while (true)
            {
                await Task.Delay(2000);
                var cpuUsage = await GetCpuUsageForProcessAsync();

                //Console.WriteLine(cpuUsage);
            }

        }
        public static void ConsumeCPU(int percentage)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                if (watch.ElapsedMilliseconds > percentage)
                {
                    Thread.Sleep(100 - percentage);
                    watch.Reset();
                    watch.Start();
                }
            }
        }

        private static async Task<double> GetCpuUsageForProcessAsync()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;

            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            Sys.CPU = cpuUsageTotal * 100;
            //Console.WriteLine($"CPU Usage: {CPU:F2} %");
            return cpuUsageTotal * 100;
        }
        private static double GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            Task.Delay(500);
            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;

            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }
    }
}
