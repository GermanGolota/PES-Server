using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands.User.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, CommandResponse>
    {
        private readonly IUserRepo _repo;

        public LogoutHandler(IUserRepo repo)
        {
            _repo = repo;
        }

        public async Task<CommandResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await CommandRunner.Run(request, async request=>
            {
                await _repo.Logout(request.UserId);
            }, "Successfully logged out user");
        }
    }
}
