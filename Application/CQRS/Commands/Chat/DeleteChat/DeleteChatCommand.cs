using MediatR;
using System;

namespace Application.CQRS.Commands
{
    public class DeleteChatCommand : IRequest<bool>
    {
        public Guid ChatId { get; set; }
        //user who tries to delete chat
        public Guid UserId { get; set; }
    }
}
