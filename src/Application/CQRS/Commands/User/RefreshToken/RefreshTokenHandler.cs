using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs.Response;
using MediatR;

namespace Application.CQRS.Commands;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IJWTokenManager _jwTokenManager;

    public RefreshTokenHandler(IJWTokenManager jwTokenManager)
    {
        _jwTokenManager = jwTokenManager;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        RefreshTokenResponse response = new();
        try
        {
            JWTokenModel token = await _jwTokenManager.Refresh(request.Token, request.RefreshToken, cancellationToken);
            response.Successfull = true;
            response.Token = token;
        }
        catch
        {
            response.Successfull = false;
        }

        return response;
    }
}