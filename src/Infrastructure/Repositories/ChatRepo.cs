using Application.Contracts.Repositories;
using Application.DTOs;
using Core;
using Core.Entities;
using Core.Exceptions;
using Infrastructure.Extensions;
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

        public async Task<Guid> CreateChat(Guid adminId, string chatName, string chatPassword)
        {
            Guid chatId = Guid.NewGuid();
            AdminToChat admin = new AdminToChat
            {
                ChatId = chatId,
                UserId = adminId
            };
            Chat chat = new Chat
            {
                ChatName = chatName,
                ChatPassword = chatPassword,
                Admins = new List<AdminToChat>
                {
                    admin
                }
            };

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return chatId;
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

        public async Task<ChatDisplayModel> GetChatModel(Guid chatId)
        {
            var chat = await _context.Chats
                .Include(x => x.Messages).ThenInclude(x => x.User)
                .Where(x => x.ChatId.Equals(chatId))
                .Select(x => new ChatDisplayModel
                {
                    ChatName = x.ChatName,
                    Messages = (List<MessageModel>)x.Messages
                    .OrderByDescending(message => message.LastEditedDate)
                    .Select(message => new MessageModel
                    {
                        Message = message.Text,
                        Username = message.User.Username
                    })
                })
                .FirstOrDefaultAsync();

            if (chat is null)
            {
                throw new NoChatException();
            }

            return chat;
        }

        public async Task<List<ChatInfoModel>> GetChats(ChatSelectionOptions options)
        {
            int count = options.ChatsPerPage;
            bool takeAll = count.Equals(-1);

            string term = options.SearchTerm;
            bool takeAny = String.IsNullOrEmpty(term);

            var query = _context.Chats
                .AsNoTracking()
                .Include(x => x.Admins)
                .Include(x => x.Users)
                .AsQueryable();

            if (!takeAll)
            {
                int numberOfPrecidingPages = options.PageNumber - 1;
                int skipCount = options.ChatsPerPage * numberOfPrecidingPages;

                query = query.OrderBy(x => x.ChatName);
                query = query.Skip(skipCount);
                query = query.Take(count);
            }

            if (!takeAny)
            {
                query = query.Where(x => EF.Functions.Like(x.ChatName, $"%{term}%"));
            }

            var result = query.MapChatsToInfoModels();

            return await result.ToListAsync();
        }

        public async Task<List<Guid>> GetChatsOfUser(Guid userId)
        {
            return await _context.Chats
                .Include(x => x.Users)
                .Include(x => x.Admins)
                .ContainsUser(userId)
                .Select(x => x.ChatId)
                .ToListAsync();
        }
    }
}
