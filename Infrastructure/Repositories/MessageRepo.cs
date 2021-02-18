using Application.Contracts;
using Core;
using Core.Entities;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
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
        public async Task AddMessageToChat(Message message)
        {
            await _context.Messages.AddAsync(message);

            await _context.SaveChangesAsync();
        }

        public async Task EditMessage(Guid messageId, string newText)
        {
            var message = _context.Messages.Find(messageId);

            if(message.IsNotNull())
            {
                message.Text = newText;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<Guid> FindUserMessageInChat(Guid userId, Guid chatId)
        {
            var message = await _context.Messages.Where(x => x.ChatId.Equals(chatId) && x.UserId.Equals(userId)).FirstOrDefaultAsync();

            if(message is null)
            {
                throw new Exception("No such message");
            }

            return message.MessageId;
        }
    }
}
