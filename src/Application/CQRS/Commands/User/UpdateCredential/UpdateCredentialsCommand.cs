using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands;

public class UpdateCredentialsCommand : IRequest<CommandResponse>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string PesKey { get; set; }
    public Guid UserId { get; set; }
}