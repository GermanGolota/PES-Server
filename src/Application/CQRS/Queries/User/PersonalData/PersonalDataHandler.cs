using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using Application.DTOs.User;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class PersonalDataHandler : IRequestHandler<PersonalDataCommand, UserProfileModel>
    {
        private readonly IUserRepo _repo;

        public PersonalDataHandler(IUserRepo repo)
        {
            _repo = repo;
        }

        public async Task<UserProfileModel> Handle(PersonalDataCommand request, CancellationToken cancellationToken)
        {
            string userName = await _repo.GetUsersUsername(request.UserId);
            return new UserProfileModel
            {
                Username = userName
            };
        }
    }
}
