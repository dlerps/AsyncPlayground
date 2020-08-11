using System;
using System.Threading.Tasks;
using ErrorHandling.Services;
using ErrorHandling.Utils;

namespace ErrorHandling.Scenarios
{
    public static class AggregateScenario
    {
        private static FailingService FailingService { get; } = new FailingService();
        
        public static async Task RunAggregateAwaited()
        {
            PlaygroundUtils.Start("Aggregate");
            
            var tasks = new[]
            {
                FailingService.ThrowAfter(200),
                FailingService.ThrowSomethingAwaited("instant awaited"),
                FailingService.ThrowAfter(120)
            };

            var whenAll = Task.WhenAll(tasks);

            try
            {
                await whenAll;
            }
            catch (Exception e)
            {
                HandleAggregateTaskException(whenAll, e);
            }
        }
        
        public static async Task RunAggregateNonAwaited()
        {
            PlaygroundUtils.Start("Aggregate Non-Awaited");
            
            Task whenAll = null;

            try
            {
                var tasks = new[]
                {
                    FailingService.ThrowSomething("instant awaited"),
                    FailingService.ThrowAfter(120)
                };

                whenAll = Task.WhenAll(tasks);
                await whenAll;
            }
            catch (Exception e)
            {
                HandleAggregateTaskException(whenAll, e);
            }
        }

        private static void HandleAggregateTaskException(Task whenAllTask, Exception exception)
        {
            if (whenAllTask?.Exception != null)
            {
                PlaygroundUtils.PrintExceptionInfo(whenAllTask.Exception);

                Console.WriteLine($"Collected Exceptions ({whenAllTask.Exception?.InnerExceptions.Count}):");

                foreach (var inner in whenAllTask.Exception!.InnerExceptions)
                    PlaygroundUtils.PrintExceptionInfo(inner);
            }
            
            if (exception != null)
            {
                Console.WriteLine("Thrown directly:");
                PlaygroundUtils.PrintExceptionInfo(exception);
            }

            if (whenAllTask?.Exception == null && exception == null)
            {
                Console.WriteLine("No exception caught.......");
            }
        }
    }
}