using Application.DTOs;
using MediatR;
using System;

namespace Application.CQRS.Queries
{
    public class GetChatsQuery : IRequest<ChatsModel>
    {
        public ChatSelectionOptions Options { get; set; }
        public Guid UserId { get; set; }
    }
}
