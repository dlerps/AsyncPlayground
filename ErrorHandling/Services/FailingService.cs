using System.Threading.Tasks;
using ErroHandling.Exceptions;

namespace ErrorHandling.Services
{
    public class FailingService
    {
        public Task ThrowSomething(string payload)
        {
            throw new AsyncPlaygroundException(payload);
        }

        public async Task ThrowSomethingAwaited(string payload)
        {
            await ThrowSomething(payload);
        }

        public async Task ThrowAfter(int ms)
        {
            await Task.Delay(ms);
            await ThrowSomething($"thrown after {ms} ms");
        }

        public async void ThrowVoid()
        {
            await ThrowSomething("From void");
        }
    }
}