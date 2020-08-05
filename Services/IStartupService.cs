using System.Threading;
using System.Threading.Tasks;

namespace Policies.Services
{
    public interface IStartupService
    {
        Task Start(CancellationToken token);
    }
}