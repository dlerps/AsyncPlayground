using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Policies.Services
{
    public class FlakyStartupService : IFlakyStartupService
    {
        private static readonly Random _random = new Random((int) DateTime.Now.Ticks);

        private readonly int _delay = 50;

        private readonly int _randomness = 15;

        private int _startupAttempts;

        public FlakyStartupService(ILogger<FlakyStartupService> logger)
        {
            logger.LogInformation("Hello from flaky service");
        }

        public async Task Start(CancellationToken token)
        {
            await Task.Yield();
            
            _startupAttempts++;
            
            if (_random.Next(_randomness) != 1)
                throw new FlakyStartupException(_startupAttempts);
            
            Console.WriteLine($"FLAKY startup service finally got started after {_startupAttempts} attempts...");
            //return Task.CompletedTask;
        }
    }
}