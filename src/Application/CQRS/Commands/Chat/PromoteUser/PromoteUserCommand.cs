﻿using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands;

public class PromoteUserCommand : IRequest<CommandResponse>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public Guid RequesterId { get; set; }
}