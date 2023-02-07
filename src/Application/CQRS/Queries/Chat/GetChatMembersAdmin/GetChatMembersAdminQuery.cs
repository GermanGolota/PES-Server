using System;
using System.Collections.Generic;
using Application.DTOs.Chat;
using MediatR;

namespace Application.CQRS.Queries;

public class GetChatMembersAdminQuery : IRequest<List<ChatMemberModelAdmin>>
{
    public Guid ChatId { get; set; }

    //user, who asks for members
    public Guid UserId { get; set; }
}