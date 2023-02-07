using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands;

public class PostMessageCommand : IRequest<CommandResponse>
{
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
    public string Message { get; set; }
}