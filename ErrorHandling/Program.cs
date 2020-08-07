using System;
using System.Threading.Tasks;
using ErrorHandling.Scenarios;
using ErrorHandling.Services;

namespace ErrorHandling
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // await AwaitScenario.RunSingleTask();
            // await AwaitScenario.RunWhenAll();
            // await AwaitScenario.RunMultipleTasks();
            // await AwaitScenario.RunSingleVoidTask();

            await AggregateScenario.RunAggregateAwaited();
            await AggregateScenario.RunAggregateNonAwaited();
        }
    }
}
