﻿using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Commands
{
    public class LogoutCommand : IRequest<CommandResponse>
    {
        public Guid UserId { get; set; }
    }
}
