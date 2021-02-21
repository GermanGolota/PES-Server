using Application.DTOs;
using Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IUserRepo
    {
        Task<User> FindUserById(string id);
        Task<User> FindUserByUsername(string username);
        Task AddUser(UserRegistrationModel user);
        Task<bool> CheckIfUsernameIsTaken(string username, CancellationToken cancellation);
        Task RemoveUser(string id);
    }
}
