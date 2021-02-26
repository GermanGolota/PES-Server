using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Commands
{
    public class PromoteUserCommand : IRequest<CommandResponse>
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public Guid RequesterId { get; set; }
    }
}
