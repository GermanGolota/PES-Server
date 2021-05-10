using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Queries
{
    public class GetMyChatsQuery : IRequest<ChatsModel>
    {
        public Guid UserId { get; set; }
        public ChatSelectionOptions Options { get; set; }
    }
}
