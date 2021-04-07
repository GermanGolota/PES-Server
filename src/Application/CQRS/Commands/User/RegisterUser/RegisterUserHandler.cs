using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.DTOs;
using Core.Entities;
using MediatR;

namespace Application.CQRS.Commands
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly IUserRepo _repo;
        private readonly IJWTokenManager _tokenManager;

        public RegisterUserHandler(IUserRepo identity, IJWTokenManager jWTokenManager)
        {
            this._repo = identity;
            this._tokenManager = jWTokenManager;
        }


        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            //TODO:Automapper
            UserRegistrationModel user = new UserRegistrationModel
            {
                Username = request.Username,
                Password = request.Password,
                PesKey = request.PesKey
            };
            await _repo.AddUser(user);

            string response = await _tokenManager.Authorize(user.Username, user.Password, cancellationToken);

            return response;
        }
    }
}
