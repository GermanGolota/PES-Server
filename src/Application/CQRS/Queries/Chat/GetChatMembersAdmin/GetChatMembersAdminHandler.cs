using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs.Chat;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Queries
{
    public class GetChatMembersAdminHandler : IRequestHandler<GetChatMembersAdminQuery, List<ChatMemberModelAdmin>>
    {
        private readonly IChatRepo _repo;

        public GetChatMembersAdminHandler(IChatRepo repo)
        {
            this._repo = repo;
        }
        public async Task<List<ChatMemberModelAdmin>> Handle(GetChatMembersAdminQuery request, 
            CancellationToken cancellationToken)
        {
            Guid chatId = request.ChatId;
            Guid userId = request.UserId;
            if(await UserIsAdmin(userId, chatId))
            {
                return await _repo.GetChatMembersAdmin(chatId);
            }
            throw new UnauthorizedException();
        }
        private async Task<bool> UserIsAdmin(Guid userId, Guid chatId)
        {
            var user = await _repo.GetMember(chatId, userId);
            return user.IsAdmin;
        }
    }
}
