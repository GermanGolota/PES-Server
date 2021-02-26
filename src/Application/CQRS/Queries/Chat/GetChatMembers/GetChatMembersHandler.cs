using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs.Chat;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Queries.Chat.GetChatMembers
{
    public class GetChatMembersHandler : IRequestHandler<GetChatMembersQuery, List<ChatMemberModel>>
    {
        private readonly IChatRepo _repo;

        public GetChatMembersHandler(IChatRepo repo)
        {
            this._repo = repo;
        }
        public async Task<List<ChatMemberModel>> Handle(GetChatMembersQuery request, CancellationToken cancellationToken)
        {
            if (await UserIsInChat(request.ChatId, request.UserId))
            {
                return await _repo.GetChatMembers(request.ChatId);
            }

            throw new UnauthorizedException();
        }
        private async Task<bool> UserIsInChat(Guid chatId, Guid userId)
        {
            await _repo.GetMember(chatId, userId);
            return true;
        }
    }
}
