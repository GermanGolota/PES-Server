using Application.DTOs.Chat;
using MediatR;
using System;
using System.Collections.Generic;

namespace Application.CQRS.Queries
{
    public class GetChatMembersQuery : IRequest<List<ChatMemberModel>>
    {
        public Guid ChatId { get; set; }
        //user, who asks for members
        public Guid UserId { get; set; }
    }
}
