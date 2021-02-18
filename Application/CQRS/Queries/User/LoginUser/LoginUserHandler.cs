using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using MediatR;

namespace Application.CQRS.Queries.User
{
    public class LoginUserHandler : IRequestHandler<LoginUserQuery, string>
    {
        private readonly IJWTokenManager _tokenManager;

        public LoginUserHandler(IJWTokenManager tokenManager)
        {
            this._tokenManager = tokenManager;
        }
        public async Task<string> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            string result = await _tokenManager.Authorize(request.Username, request.Password, cancellationToken);
            return result;
        }
    }
}
