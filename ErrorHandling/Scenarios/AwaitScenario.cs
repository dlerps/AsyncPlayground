using System;
using System.Threading.Tasks;
using ErrorHandling.Services;
using ErrorHandling.Utils;

namespace ErrorHandling.Scenarios
{
    public static class AwaitScenario
    {
        private static FailingService FailingService { get; } = new FailingService();

        public static async Task RunWhenAll()
        {
            PlaygroundUtils.Start("When-All");
            
            try
            {
                var taskList = new[]
                {
                    Task.Run(() => RunAsync(1)),
                    Task.Run(() => RunAsync(2)),
                    Task.Run(() => RunAsync(3)),
                };

                // throws exception with i=1
                await Task.WhenAll(taskList);
            }
            catch (Exception e)
            {
                PlaygroundUtils.PrintExceptionInfo(e);
            }
        }
        
        public static async Task RunSingleTask()
        {
            PlaygroundUtils.Start("Single");

            try
            {
                // throws
                await RunAsync(0);
            }
            catch (Exception e)
            {
                PlaygroundUtils.PrintExceptionInfo(e);
            }
        }
        
        public static Task RunSingleVoidTask()
        {
            PlaygroundUtils.Start("Single-Void");

            try
            {
                // throws
                FailingService.ThrowVoid();
            }
            catch (Exception e)
            {
                PlaygroundUtils.PrintExceptionInfo(e);
            }

            return Task.CompletedTask;
        }

        public static async Task RunMultipleTasks()
        {
            PlaygroundUtils.Start("Multiple");

            try
            {
                // throws exception with ms=50
                var task1 = FailingService.ThrowAfter(10);
                var task2 = FailingService.ThrowAfter(50);
                
                await task2;
                await task1;
            }
            catch (Exception e)
            {
                PlaygroundUtils.PrintExceptionInfo(e);
            }
        }

        private static async Task RunAsync(int i)
        {
            await FailingService.ThrowSomething($"async non-awaited number {i}");
        }
    }
}