using Application.DTOs;
using Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IUserRepo
    {
        Task<User> FindUserByUsername(string username);
        Task AddUser(UserRegistrationModel user);
        Task<bool> CheckIfUsernameIsTaken(string username, CancellationToken cancellation);
        Task RemoveUser(Guid id);
        Task<string> GetUsersUsername(Guid id);
    }
}
