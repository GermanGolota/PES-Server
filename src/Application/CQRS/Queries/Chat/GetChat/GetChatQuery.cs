using Application.DTOs;
using MediatR;
using System;

namespace Application.CQRS.Queries
{
    public class GetChatQuery : IRequest<ChatDisplayModel>
    {
        public string ChatId { get; set; }
        public Guid RequesterId { get; set; }
    }
}
