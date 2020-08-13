using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cancellation.Services
{
    public class ClockService
    {
        public async Task WhatsTheTime(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"{DateTime.Now:hh:mm:ss}");
                
                // don't pass the token and use the loop condition to exit
                //await Task.Delay(1000);

                // pass the token into the delay method -> causes an exception on cancel which needs to be handled
                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException oce)
                {
                    Console.WriteLine($"Cancellation caused {oce.GetType().Name}. Exiting now!");
                    return;
                }
            }
            
            Console.WriteLine("Cancellation requested... stopping now");
        }
    }
}