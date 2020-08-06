using System;
using System.Threading;
using System.Threading.Tasks;

namespace Policies.Services
{
    public class QuickStartupService : IQuickStartupService
    {
        private readonly int _delay = 20;
        
        public async Task Start(CancellationToken token)
        {
            await Task.Delay(_delay, token);
            Console.WriteLine("QUICK reporting for duty!");
        }
    }
}