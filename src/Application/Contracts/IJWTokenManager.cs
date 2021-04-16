using Application.DTOs.Response;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IJWTokenManager
    {
        Task<JWTokenModel> Authorize(string username, string password, CancellationToken cancellation);
    }
}