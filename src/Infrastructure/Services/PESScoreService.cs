using Application.Contracts;
using Core;
using Core.Exceptions;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Application.DTOs.Service;

namespace Infrastructure.Services
{
    public class PesScoreService : IPesScoreService
    {
        private readonly PESContext _context;

        public PesScoreService(PESContext context)
        {
            _context = context;
        }

        public async Task<PesScoreModel> GetPesScoreFor(string username)
        {
            var userId = await _context.Users
                .AsNoTracking()
                .Where(x => x.Username.Equals(username))
                .Select(x => x.UserId)
                .FirstOrDefaultAsync();

            return await GetPesScore(userId);
        }

        public async Task<PesScoreModel> GetPesScoreFor(Guid userId)
        {
            return await GetPesScore(userId);
        }

        private async Task<PesScoreModel> GetPesScore(Guid userId)
        {
            if (NoUser(userId))
            {
                throw new NoUserException();
            }

            var pesKey = await _context.Users
                .AsNoTracking()
                .Where(x => x.UserId.Equals(userId))
                .Select(x => x.PESKey)
                .FirstOrDefaultAsync();

            var messages = await _context.Messages
                   .AsNoTracking()
                   .Where(x => x.UserId.Equals(userId))
                   .Select(x => x.Text)
                   .ToListAsync();

            return new PesScoreModel
            {
                Messages = messages,
                PesKey = pesKey
            };
        }

        private bool NoUser(Guid userId)
        {
            return userId == default;
        }
    }
}
