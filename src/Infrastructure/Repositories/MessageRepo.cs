using Application.Contracts.Repositories;
using Core;
using Core.Entities;
using Core.Exceptions;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MessageRepo : IMessageRepo
    {
        private readonly PESContext _context;

        public MessageRepo(PESContext context)
        {
            this._context = context;
        }

        public async Task AddMessageToChat(string messageText, Guid chatId, Guid userId)
        {
            Message message = new Message
            {
                ChatId = chatId,
                UserId = userId,
                Text = messageText,
                LastEditedDate = DateTime.UtcNow,
                MessageId = Guid.NewGuid()
            };

            await AddMessageToChat(message);
        }
        private async Task AddMessageToChat(Message message)
        {
            await _context.Messages.AddAsync(message);

            await _context.SaveChangesAsync();
        }


        public async Task DeleteMessage(Guid userId, Guid chatId)
        {
            var message = await _context.Messages
                //this check can't be separated into a function, becouse it would
                //result in a client-side evaluation
                .Where(x => x.UserId.Equals(userId) && x.ChatId.Equals(chatId))
                .FirstOrDefaultAsync();

            if (message is null)
            {
                throw new NoMessageException();
            }

            _context.Messages.Remove(message);

            await _context.SaveChangesAsync();

        }

        public async Task EditMessage(Guid userId, Guid chatId, string newText)
        {
            var message = _context.Messages
                .Where(x => x.UserId.Equals(userId) && x.ChatId.Equals(chatId))
                .FirstOrDefault();

            if (message.IsNotNull())
            {
                message.Text = newText;

                message.LastEditedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<Guid> FindUserMessageInChat(Guid userId, Guid chatId)
        {
            var message = await _context.Messages.Where(x => x.ChatId.Equals(chatId) && x.UserId.Equals(userId)).FirstOrDefaultAsync();

            if(message is null)
            {
                throw new NoMessageException();
            }

            return message.MessageId;
        }

        public async Task<List<string>> GetAllUserMessages(Guid userId)
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(x => x.UserId.Equals(userId))
                .Select(x=>x.Text)
                .ToListAsync();
        }
    }
}
