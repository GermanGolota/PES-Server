using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.Contracts.Service;
using Application.DTOs.UpdateMessages;

namespace Application.CQRS.Commands;

public class AddUserToChatHandler : PesCommand<AddUserToChatCommand>
{
    private readonly IChatMembersService _membersService;
    private readonly IMessageSender _sender;
    private readonly IUserRepo _userRepo;

    public AddUserToChatHandler(IUserRepo userRepo, IMessageSender sender,
        IChatMembersService membersService)
    {
        _userRepo = userRepo;
        _sender = sender;
        _membersService = membersService;
    }

    public override string SuccessMessage => "Succesfully added user to chat";

    public override async Task Run(AddUserToChatCommand request, CancellationToken token)
    {
        await _membersService.AddUser(request.ChatId, request.UserId, request.Password);
        await SendUpdateMessage(request);
    }

    private async Task SendUpdateMessage(AddUserToChatCommand request)
    {
        string userName = await _userRepo.GetUsersUsername(request.UserId);
        UserJoinedChatUpdateMessage updateMessage = UpdateMessageFactory.CreateUserJoinedUpdate(userName);
        await _sender.SendMessageToSocket(updateMessage, request.ChatId);
    }
}