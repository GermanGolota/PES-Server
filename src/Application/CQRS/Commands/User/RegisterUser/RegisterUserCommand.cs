using Application.DTOs.Response;
using MediatR;

namespace Application.CQRS.Commands;

public class RegisterUserCommand : IRequest<JWTokenModel>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string PesKey { get; set; }
}