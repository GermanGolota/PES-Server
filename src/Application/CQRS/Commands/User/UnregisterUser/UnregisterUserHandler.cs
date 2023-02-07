using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Service;

namespace Application.CQRS.Commands;

public class UnregisterUserHandler : PesCommand<UnregisterUserCommand>
{
    private readonly IChatMembersService _chatMembersService;
    private readonly IUserRepo _repo;

    public UnregisterUserHandler(IUserRepo repo, IChatMembersService chatMembersService)
    {
        _repo = repo;
        _chatMembersService = chatMembersService;
    }

    public override string SuccessMessage => "Successfully removed user";

    public override async Task Run(UnregisterUserCommand request, CancellationToken token)
    {
        Guid id = request.UserId;
        await _repo.RemoveUser(id);
        await _chatMembersService.RemoveUserFromAllChats(id);
    }
}