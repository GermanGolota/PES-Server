using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands
{
    public class PostMessageCommand : IRequest<PostMessageResponse>
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
        public string Message { get; set; }
    }
}
