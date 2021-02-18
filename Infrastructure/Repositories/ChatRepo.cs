using Application.Contracts;
using Application.DTOs;
using Core;
using Core.Entities;
using Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

            if (chat is null)
            {
                throw new NoChatException();
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
            var chat = await _context.Chats
                .Include(x => x.Messages).ThenInclude(x=>x.User)
                .FirstAsync(x => x.ChatId.Equals(chatId));

            if (chat is null)
            {
                throw new NoChatException();
            }

            List<MessageModel> messages = new List<MessageModel>();

            foreach (var message in chat.Messages)
            {
                messages.Add(new MessageModel
                {
                    Message = message.Text,
                    Username = message.User.Username
                });
            }

            ChatDisplayModel output = new ChatDisplayModel
            {
                ChatName = chat.ChatName,
                Messages = messages
            };

            return output;
        }

        public async Task<IEnumerable<Chat>> GetChats(ChatSelectionOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
