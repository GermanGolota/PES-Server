using Application.DTOs;
using Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Commands
{
    public class PersonalDataCommand : IRequest<UserProfileModel>
    {
        public Guid UserId { get; set; }
    }
}
