using System;
using System.Threading;
using System.Threading.Tasks;
using Cancellation.Services;

namespace Cancellation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var clock = new ClockService();
            var cts = new CancellationTokenSource();
            var timeout = new Random().Next(5000, 15000);
            
            // Starting the clock and cancel timer
            var stopTask = CancelAfter(timeout, cts);
            var clockTask = clock.WhatsTheTime(cts.Token);

            // wait for everything to complete gracefully
            await Task.WhenAll(new[] { clockTask, stopTask });
        }

        private static async Task CancelAfter(int timeout, CancellationTokenSource cts)
        {
            Console.WriteLine($"This clock will stop running in {timeout / 1000} sec");

            await Task.Delay(timeout);

            cts.Cancel();
        }
    }
}
