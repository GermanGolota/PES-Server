using System;
using System.ComponentModel.DataAnnotations;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands;

public class RegisterChatCommand : IRequest<CommandResponse>
{
    [Required] public string ChatName { get; set; }

    public bool IsMultiMessage { get; set; }
    public Guid AdminId { get; set; }
    public string ChatPassword { get; set; }
}