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

        public async Task<Guid> CreateChat(Guid adminId, string chatName, string chatPassword, bool isMultiMessage)
        {
            Guid chatId = Guid.NewGuid();
            UserToChat admin = new UserToChat
            {
                ChatId = chatId,
                UserId = adminId,
                Role = Role.Creator
            };
            Chat chat = new Chat
            {
                ChatName = chatName,
                ChatPassword = chatPassword,
                IsMultiMessage = isMultiMessage,
                Users = new List<UserToChat>
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
                    IsMultiMessage = x.IsMultiMessage,
                    Messages = (List<MessageModel>)x.Messages
                    .OrderByDescending(message => message.LastEditedDate)
                    .Select(message => new MessageModel
                    {
                        Message = message.Text,
                        Username = message.User.Username,
                        MessageId = message.MessageId
                    })
                })
                .FirstOrDefaultAsync();

            if (chat is null)
            {
                throw new NoChatException();
            }

            return chat;
        }

        public async Task<List<ChatInfoModel>> GetChats(ChatSelectionOptions options, Guid memberId)
        {
            var query = _context.Chats
                .AsNoTracking()
                .Include(x => x.Users)
                .AsQueryable();

            return await GetSelectionForUserFromQuery(options, memberId, query);
        }

        public async Task<List<ChatInfoModel>> GetMyChats(ChatSelectionOptions options, Guid memberId)
        {
            var query = _context.Chats
             .AsNoTracking()
             .Include(x => x.Users)
             .Where(x => x.Users.Where(x => x.UserId.Equals(memberId)).Count() > 0);

            return await GetSelectionForUserFromQuery(options, memberId, query);
        }

        private async Task<List<ChatInfoModel>> GetSelectionForUserFromQuery(ChatSelectionOptions options,
            Guid memberId, IQueryable<Chat> query)
        {
            query = ApplySearchOptionsToQuery(options, query);

            var result = query.MapChatsToInfoModels(memberId);

            var pre = await result.ToListAsync();
            return CalculateRole(memberId, pre);
        }

        private List<ChatInfoModel> CalculateRole(Guid memberId, List<PreChatInfoModel> pre)
        {
            return pre.Select(x => new ChatInfoModel
            {
                ChatId = x.ChatId,
                ChatName = x.ChatName,
                UserCount = x.Users.Count,
                Role = x.Users.Where(x => x.UserId.Equals(memberId)).FirstOrDefault()?.Role
            }).ToList();
        }

        private IQueryable<Chat> ApplySearchOptionsToQuery(ChatSelectionOptions options, IQueryable<Chat> query)
        {
            int count = options.ChatsPerPage;
            bool takeAll = count.Equals(-1);

            string term = options.SearchTerm;
            bool takeAny = String.IsNullOrEmpty(term);

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

            return query;
        }


        public async Task<List<Guid>> GetChatsOfUser(Guid userId)
        {
            return await _context.Chats
                .Include(x => x.Users)
                .ContainsUser(userId)
                .Select(x => x.ChatId)
                .ToListAsync();
        }
    }
}
