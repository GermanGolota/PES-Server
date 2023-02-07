using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.Contracts.Service;
using Application.DTOs.UpdateMessages;

namespace Application.CQRS.Commands;

public class DeleteChatHandler : PesCommand<DeleteChatCommand>
{
    private readonly IChatMembersService _membersService;
    private readonly IChatRepo _repo;
    private readonly IMessageSender _sender;
    private readonly IUserRepo _userRepo;

    public DeleteChatHandler(IChatRepo repo, IUserRepo userRepo, IMessageSender sender,
        IChatMembersService membersService)
    {
        _repo = repo;
        _userRepo = userRepo;
        _sender = sender;
        _membersService = membersService;
    }

    public override string SuccessMessage => "Successfully deleted chat";

    public override async Task<bool> Authorize(DeleteChatCommand request, CancellationToken token)
    {
        Guid creatorId = await _membersService.GetChatCreator(request.ChatId);
        return creatorId.Equals(request.UserId);
    }

    public override async Task Run(DeleteChatCommand request, CancellationToken token)
    {
        await _repo.DeleteChat(request.ChatId);
        await SendUpdateMessage(request);
    }

    private async Task SendUpdateMessage(DeleteChatCommand request)
    {
        string userName = await _userRepo.GetUsersUsername(request.UserId);
        ChatDeletedUpdateMessage updateMessage = UpdateMessageFactory.CreateChatDeletedMessage(userName);
        await _sender.SendMessageToSocket(updateMessage, request.ChatId);
    }
}