using System;
using System.Threading.Tasks;
using ErrorHandling.Services;
using ErrorHandling.Utils;

namespace ErrorHandling.Scenarios
{
    public static class AggregateScenario
    {
        private static FailingService FailingService { get; } = new FailingService();
        
        public static async Task RunAggregate()
        {
            PlaygroundUtils.Start("Aggregate");
            
            var tasks = new[]
            {
                FailingService.ThrowSomething("normal"),
                FailingService.ThrowAfter(120)
            };

            var whenAll = Task.WhenAll(tasks);

            try
            {
                await whenAll;
            }
            catch
            {
                PlaygroundUtils.PrintExceptionInfo(whenAll.Exception);
                
                Console.WriteLine($"Collected Exceptions ({whenAll.Exception?.InnerExceptions.Count}):");
                
                foreach (var inner in whenAll.Exception!.InnerExceptions)
                    PlaygroundUtils.PrintExceptionInfo(inner);
            }
        }
    }
}