using System;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Core.Entities;

namespace Application.Contracts.Repositories;

public interface IUserRepo
{
    Task<User> FindUserByUsername(string username);
    Task AddUser(UserRegistrationModel user);
    Task<bool> CheckIfUsernameIsTaken(string username, CancellationToken cancellation);
    Task RemoveUser(Guid id);
    Task<string> GetUsersUsername(Guid id);

    Task<string> GetRefreshTokenFor(Guid userId);
    Task SetRefreshTokenFor(Guid userId, string refreshToken);
    Task UpdateCredentials(Guid userId, string newUsername, string newPassword, string newPesKey);
    Task Logout(Guid userId);
}