using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands
{
    public class EditMessageCommand: IRequest<EditMessageResponse>
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
        public string UpdatedMessage { get; set; }
    }
}
