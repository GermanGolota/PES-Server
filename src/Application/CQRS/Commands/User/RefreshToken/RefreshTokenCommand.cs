using Application.DTOs.Response;
using MediatR;

namespace Application.CQRS.Commands;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}