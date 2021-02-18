using MediatR;

namespace Application.CQRS.Queries
{
    public class LoginUserQuery : IRequest<string>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
