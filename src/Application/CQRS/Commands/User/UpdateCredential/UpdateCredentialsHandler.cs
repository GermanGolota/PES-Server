using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class UpdateCredentialsHandler : IRequestHandler<UpdateCredentialsCommand, CommandResponse>
    {
        private readonly IUserRepo _repo;

        public UpdateCredentialsHandler(IUserRepo repo)
        {
            _repo = repo;
        }

        public async Task<CommandResponse> Handle(UpdateCredentialsCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response;
            try
            {
                await _repo.UpdateCredentials(request.UserId, request.Username, request.Password, request.PesKey);
                response = CommandResponse.CreateSuccessfull("Successfully updated credentials");
            }
            catch(Exception exc)
            {
                if(exc is ExpectedException)
                {
                    response = CommandResponse.CreateUnsuccessfull(exc.Message);
                }
                else
                {
                    response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.ServerError);
                }
            }
            return response;
        }
    }
}
