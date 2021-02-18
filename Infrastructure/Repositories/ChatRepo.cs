using Application.Contracts;
using Application.DTOs;
using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ChatRepo : IChatRepo
    {
        private readonly PESContext _context;

        public ChatRepo(PESContext context)
        {
            this._context = context;
        }
        public async Task CreateChat(Chat chat, User admin)
        {
            chat.Admins = new List<AdminToChat>();
            var adminToChat = new AdminToChat
            {
                ChatId = chat.ChatId,
                UserId = admin.UserId
            };
            chat.Admins.Add(adminToChat);

            await _context.Chats.AddAsync(chat);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteChat(Guid chatId)
        {
            Chat chat = await _context.Chats.FindAsync(chatId);

            if(chat is null)
            {
                throw new Exception("No such chat"); 
            }

            _context.Chats.Remove(chat);

            await _context.SaveChangesAsync();
        }

        public async Task<Chat> GetChatById(Guid chatId)
        {
            return await _context.Chats.FindAsync(chatId);
        }

        public async Task<ChatDisplayModel> GetChatModel(Guid chatId)
        {
            var chat = await _context.Chats.Include(x=>x.Messages).FirstAsync(x=>x.ChatId.Equals(chatId));

            if(chat is null)
            {
                //todo:create exceptions
                throw new Exception("No such chat");
            }

            List<Guid> userIds = new List<Guid>();
            foreach (var message in chat.Messages)
            {
                userIds.Add(message.UserId);
            }
        }

        public async Task<IEnumerable<Chat>> GetChats(ChatSelectionOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
