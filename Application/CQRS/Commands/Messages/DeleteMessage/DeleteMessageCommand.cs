using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Commands
{
    public class DeleteMessageCommand : IRequest<CommandResponse>
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
    }
}
