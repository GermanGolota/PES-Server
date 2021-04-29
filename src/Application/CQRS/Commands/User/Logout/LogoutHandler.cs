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
            CommandResponse response;
            try
            {
                await _repo.Logout(request.UserId);
                response = CommandResponse.CreateSuccessfull("Successfully logged out user");
            }
            catch (Exception exc)
            {
                string message;
                if (exc is ExpectedException)
                {
                    message = exc.Message;
                }
                else
                {
                    message = ExceptionMessages.ServerError;
                }
                response = CommandResponse.CreateUnsuccessfull(message);
            }
            return response;
        }
    }
}
