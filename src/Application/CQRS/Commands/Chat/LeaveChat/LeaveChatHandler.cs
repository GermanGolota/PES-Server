﻿using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.Contracts.Service;
using Application.DTOs.UpdateMessages;

namespace Application.CQRS.Commands;

internal class LeaveChatHandler : PesCommand<LeaveChatCommand>
{
    private readonly IChatMembersService _membersService;
    private readonly IMessageSender _sender;
    private readonly IUserRepo _userRepo;

    private string _successMessage;

    public LeaveChatHandler(IChatMembersService membersService, IMessageSender sender, IUserRepo userRepo)
    {
        _membersService = membersService;
        _sender = sender;
        _userRepo = userRepo;
    }

    public override string SuccessMessage => _successMessage;

    public override async Task Run(LeaveChatCommand request, CancellationToken token)
    {
        await _membersService.RemoveUserFromChat(request.ChatId, request.UserId);
        await SendUpdateMessage(request);
        _successMessage = $"Sucessfully left chat {request.ChatId}";
    }

    private async Task SendUpdateMessage(LeaveChatCommand request)
    {
        string userName = await _userRepo.GetUsersUsername(request.UserId);
        UserLeftChatUpdateMessage updateMessage = UpdateMessageFactory.CreateUserLeftUpdate(userName);
        await _sender.SendMessageToSocket(updateMessage, request.ChatId);
    }
}