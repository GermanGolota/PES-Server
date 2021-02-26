using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IJWTokenManager
    {
        Task<string> Authorize(string username, string password, CancellationToken cancellation);
    }
}