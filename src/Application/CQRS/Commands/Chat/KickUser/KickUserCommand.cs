using Application.DTOs;
using MediatR;
using System;

namespace Application.CQRS.Commands
{
    public class KickUserCommand : IRequest<CommandResponse>
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public Guid RequesterId { get; set; }
    }
}
