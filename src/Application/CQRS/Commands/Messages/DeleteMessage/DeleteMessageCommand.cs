using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands;

public class DeleteMessageCommand : IRequest<CommandResponse>
{
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
    public Guid MessageId { get; set; }
}