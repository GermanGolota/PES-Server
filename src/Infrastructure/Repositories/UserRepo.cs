using Application.Contracts;
using Application.DTOs;
using Core;
using Core.Entities;
using Core.Exceptions;
using Core.Extensions;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly PESContext _context;
        private readonly IEncrypter _encrypter;

        public UserRepo(PESContext context, IEncrypter encrypter)
        {
            this._context = context;
            this._encrypter = encrypter;
        }
        public async Task AddUser(UserRegistrationModel userModel)
        {
            string hash = await _encrypter.Encrypt(userModel.Password);
            User user = new User
            {
                Username = userModel.Username,
                PasswordHash = hash,
                UserId = Guid.NewGuid()
            };

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfUsernameIsTaken(string username, CancellationToken cancellation)
        {
            int userCount = await _context.Users.CountAsync(x => x.Username.Equals(username));

            if (userCount != 0)
            {
                return true;
            }

            return false;
        }

        public async Task<User> FindUserById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> FindUserByUsername(string username)
        {
            return await _context.Users.Where(x => x.Username.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<string> GetUsersUsername(Guid id)
        {
            return await _context.Users
                .Where(x => x.UserId.Equals(id))
                .Select(x=>x.Username)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveUser(Guid id)
        {
            var user = await _context.Users
                .Where(x => x.UserId.Equals(id))
                .FirstOrDefaultAsync();

            if (user is null)
            {
                throw new NoUserException();
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }
    }
}
