using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;

namespace Application.CQRS.Commands.User.Logout;

public class LogoutHandler : PesCommand<LogoutCommand>
{
    private readonly IUserRepo _repo;

    public LogoutHandler(IUserRepo repo)
    {
        _repo = repo;
    }

    public override string SuccessMessage => "Successfully logged out user";

    public override async Task Run(LogoutCommand request, CancellationToken cancellation)
    {
        await _repo.Logout(request.UserId);
    }
}