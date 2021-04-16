using Application.DTOs.Response;
using MediatR;

namespace Application.CQRS.Queries
{
    public class LoginUserQuery : IRequest<JWTokenModel>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
