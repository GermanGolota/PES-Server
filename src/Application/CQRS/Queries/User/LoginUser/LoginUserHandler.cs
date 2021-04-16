using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs.Response;
using MediatR;

namespace Application.CQRS.Queries.User
{
    public class LoginUserHandler : IRequestHandler<LoginUserQuery, JWTokenModel>
    {
        private readonly IJWTokenManager _tokenManager;

        public LoginUserHandler(IJWTokenManager tokenManager)
        {
            this._tokenManager = tokenManager;
        }
        public async Task<JWTokenModel> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            JWTokenModel result = await _tokenManager.Authorize(request.Username, request.Password, cancellationToken);
            return result;
        }
    }
}
