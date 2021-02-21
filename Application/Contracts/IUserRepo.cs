using Application.DTOs;
using Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IUserRepo
    {
        Task<User> FindUserById(Guid id);
        Task<User> FindUserByUsername(string username);
        Task AddUser(UserRegistrationModel user);
        Task<bool> CheckIfUsernameIsTaken(string username, CancellationToken cancellation);
        Task RemoveUser(Guid id);
    }
}
