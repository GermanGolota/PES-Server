using System;
using System.Collections.Generic;
using MediatR;
using System.Text;
using Application.DTOs;
using System.Threading.Tasks;
using System.Threading;
using Application.Contracts;
using Core.Exceptions;

namespace Application.CQRS.Commands
{
    public class UnregisterUserHandler : IRequestHandler<UnregisterUserCommand, CommandResponse>
    {
        private readonly IUserRepo _repo;

        public UnregisterUserHandler(IUserRepo repo)
        {
            this._repo = repo;
        }
        public async Task<CommandResponse> Handle(UnregisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var id = request.UserId;

            CommandResponse response;

            try
            {
                await _repo.RemoveUser(id);
                response = CommandResponse.CreateSuccessfull("Successfully removed user");
            }
            catch(ExpectedException exc)
            {
                response = CommandResponse.CreateUnsuccessfull(exc.Message);
            }
            catch
            {
                response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.NoUser);
            }

            return response;
        }
    }
}
