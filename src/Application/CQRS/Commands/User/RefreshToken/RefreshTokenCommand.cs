using Application.DTOs.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Commands
{
    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
