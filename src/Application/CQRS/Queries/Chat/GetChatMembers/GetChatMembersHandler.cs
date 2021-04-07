using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Service;
using Application.DTOs.Chat;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Queries.Chat.GetChatMembers
{
    public class GetChatMembersHandler : IRequestHandler<GetChatMembersQuery, List<ChatMemberModel>>
    {
        private readonly IChatMembersService _membersService;

        public GetChatMembersHandler(IChatMembersService membersService)
        {
            _membersService = membersService;
        }
        public async Task<List<ChatMemberModel>> Handle(GetChatMembersQuery request, CancellationToken cancellationToken)
        {
            if (await UserIsInChat(request.ChatId, request.UserId))
            {
                return await _membersService.GetChatMembers(request.ChatId);
            }

            throw new UnauthorizedException();
        }
        private async Task<bool> UserIsInChat(Guid chatId, Guid userId)
        {
            await _membersService.GetMember(chatId, userId);
            return true;
        }
    }
}
