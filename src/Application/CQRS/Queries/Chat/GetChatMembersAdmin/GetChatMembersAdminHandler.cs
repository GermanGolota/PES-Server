using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Service;
using Application.DTOs.Chat;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Queries;

public class GetChatMembersAdminHandler : IRequestHandler<GetChatMembersAdminQuery, List<ChatMemberModelAdmin>>
{
    private readonly IChatMembersService _membersService;

    public GetChatMembersAdminHandler(IChatMembersService membersService)
    {
        _membersService = membersService;
    }

    public async Task<List<ChatMemberModelAdmin>> Handle(GetChatMembersAdminQuery request,
        CancellationToken cancellationToken)
    {
        Guid chatId = request.ChatId;
        Guid userId = request.UserId;
        if (await UserIsAdmin(userId, chatId)) return await _membersService.GetChatMembersAdmin(chatId);
        throw new UnauthorizedException();
    }

    private async Task<bool> UserIsAdmin(Guid userId, Guid chatId)
    {
        MemberModel user = await _membersService.GetMember(chatId, userId);
        return user.IsAdmin;
    }
}