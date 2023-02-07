using System;
using Application.DTOs.User;
using MediatR;

namespace Application.CQRS.Commands;

public class PersonalDataCommand : IRequest<UserProfileModel>
{
    public Guid UserId { get; set; }
}