﻿using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands;

public class UnregisterUserCommand : IRequest<CommandResponse>
{
    public Guid UserId { get; set; }
}