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
    public class UpdateCredentialsHandler : PesCommand<UpdateCredentialsCommand>
    {
        private readonly IUserRepo _repo;

        public UpdateCredentialsHandler(IUserRepo repo)
        {
            _repo = repo;
        }

        public override string SuccessMessage => "Successfully updated credentials";

        public override async Task Run(UpdateCredentialsCommand request, CancellationToken token)
        {
            await _repo.UpdateCredentials(request.UserId, request.Username, request.Password, request.PesKey);
        }
    }
}
