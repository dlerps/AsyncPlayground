using System;
using System.Threading;
using System.Threading.Tasks;

namespace Policies.Services
{
    public class SlowStartupService : ISlowStartupService
    {
        private readonly int _delay = 5000;
        
        public async Task Start(CancellationToken token)
        {
            await Task.Delay(_delay, token);
            Console.WriteLine("SLOW startup service is ready for action!");
        }
    }
}