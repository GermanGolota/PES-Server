using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands;

public class AddUserToChatCommand : IRequest<CommandResponse>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public string Password { get; set; }
}