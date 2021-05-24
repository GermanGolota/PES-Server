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
            return await CommandRunner.Run(request, async request =>
             {
                 await _repo.UpdateCredentials(request.UserId, request.Username, request.Password, request.PesKey);
             }, "Successfully updated credentials");

        }
    }
}
