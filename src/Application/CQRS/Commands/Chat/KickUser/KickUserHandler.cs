using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.Contracts.Service;
using Application.DTOs.UpdateMessages;
using Core.Extensions;

namespace Application.CQRS.Commands;

public class KickUserHandler : PesCommand<KickUserCommand>
{
    private readonly IChatMembersService _membersService;
    private readonly IMessageSender _sender;
    private readonly IUserRepo _userRepo;

    private string _successMessage;

    public KickUserHandler(IUserRepo userRepo, IMessageSender sender, IChatMembersService membersService)
    {
        _userRepo = userRepo;
        _sender = sender;
        _membersService = membersService;
    }

    public override string SuccessMessage => _successMessage;

    public override async Task<bool> Authorize(KickUserCommand request, CancellationToken token)
    {
        List<Guid> admins = await _membersService.GetAdminsOfChat(request.ChatId);
        return admins.Contains(request.RequesterId) && admins.NotContains(request.UserId);
    }

    public override async Task Run(KickUserCommand request, CancellationToken token)
    {
        await _membersService.Kick(request.ChatId, request.UserId);
        await SendUpdateMessage(request);
        _successMessage = $"Successfully kicked user {request.UserId} in chat {request.ChatId}";
    }

    private async Task SendUpdateMessage(KickUserCommand request)
    {
        string userName = await _userRepo.GetUsersUsername(request.UserId);
        UserKickedUpdateMessage updateMessage = UpdateMessageFactory.CreateUserKickedUpdate(userName);
        await _sender.SendMessageToSocket(updateMessage, request.ChatId);
    }
}