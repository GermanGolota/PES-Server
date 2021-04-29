using Application.Contracts.Repositories;
using Application.Contracts.Service;
using Application.DTOs.Chat;
using Core;
using Core.Entities;
using Core.Exceptions;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ChatMembersService : IChatMembersService
    {
        private readonly PESContext _context;
        private readonly IChatRepo _chatRepo;

        public ChatMembersService(PESContext context, IChatRepo chatRepo)
        {
            this._context = context;
            _chatRepo = chatRepo;
        }

        public async Task AddUser(Guid chatId, Guid userId, string chatPassword)
        {
            var chat = await GetChatWithUsers(chatId);

            if (UserIsPresent(userId, chat.Users))
            {
                throw new UserAlreadyInTheChatException();
            }

            if (!chat.ChatPassword.Equals(chatPassword))
            {
                throw new IncorrectPasswordException();
            }

            var user = new UserToChat
            {
                ChatId = chatId,
                UserId = userId,
                Role = Role.User
            };

            chat.Users.Add(user);

            await _context.SaveChangesAsync();
        }
        private bool UserIsPresent(Guid userId, List<UserToChat> users)
        {
            return users.IsNotNull() && users.Where(x => x.UserId.Equals(userId)).Any();
        }
        private async Task<Chat> GetChatWithUsers(Guid chatId)
        {
            var chat = await _context.Chats
                .Include(x => x.Users)
                .Where(x => x.ChatId.Equals(chatId))
                .FirstOrDefaultAsync();

            if (chat is null)
            {
                throw new NoChatException();
            }

            return chat;
        }

        public async Task<List<Guid>> GetAdminsOfChat(Guid chatId)
        {
            var chat = await _context.Chats.Where(x => x.ChatId.Equals(chatId))
                .Include(x => x.Users)
                .FirstOrDefaultAsync();

            if (chat is null)
            {
                throw new NoChatException();
            }

            return chat.Users
                .Where(x => x.Role.Equals(Role.Admin) || x.Role.Equals(Role.Creator))
                .Select(x => x.UserId)
                .ToList();
        }

        private async Task<List<ChatMemberModelAdmin>> GetMembers(Guid chatId)
        {
            var chat = await GetChatWithUsers(chatId);
            List<Guid> admins = await GetAdminsOfChat(chatId);
            List<Guid> userIds = GetUserIds(chat);
            List<ChatMemberModelAdmin> models = await _context.Users
                .Where(x => userIds.Contains(x.UserId))
                .Select(x => new ChatMemberModelAdmin
                {
                    Username = x.Username,
                    MemberId = x.UserId,
                    IsAdmin = admins.Contains(x.UserId)
                })
                .ToListAsync();

            return models;
        }
        private List<Guid> GetUserIds(Chat chat)
        {
            return chat.Users.Select(x => x.UserId).ToList();
        }
        public async Task<List<ChatMemberModel>> GetChatMembers(Guid chatId)
        {
            var members = await GetMembers(chatId);
            return members.Select(x => new ChatMemberModel
            {
                Username = x.Username,
                IsAdmin = x.IsAdmin
            }).ToList();
        }
        public async Task<List<ChatMemberModelAdmin>> GetChatMembersAdmin(Guid chatId)
        {
            return await GetMembers(chatId);
        }

        public async Task<MemberModel> GetMember(Guid chatId, Guid userId)
        {
            var chat = await GetChatWithUsers(chatId);

            var user = chat.Users
                .Where(x => x.UserId.Equals(userId))
                .FirstOrDefault();

            if (user is null)
            {
                throw new NoUserException();
            }

            string username = _context.Users
                .Where(x => x.UserId.Equals(userId))
                .FirstOrDefault()
                ?.Username;

            MemberModel member = new MemberModel
            {
                IsAdmin = user.Role == Role.Admin || user.Role == Role.Creator,
                Username = username
            };

            return member;
        }

        public async Task PromoteToAdmin(Guid chatId, Guid userId)
        {
            var chat = await GetChatWithUsers(chatId);

            if (UserIsPresent(userId, chat.Users))
            {
                var user = chat.Users.Where(x => x.UserId.Equals(userId)).First();
                if (user.Role == Role.Admin || user.Role == Role.Creator)
                {
                    throw new CannotPromoteAdminException();
                }
                user.Role = Role.Admin;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NoUserException();
            }
        }

        public async Task RemoveUserFromChat(Guid chatId, Guid userId)
        {
            var chat = await GetChatWithUsers(userId);

            var userQuerry = chat.Users.Where(x => x.UserId.Equals(userId));

            var user = userQuerry.FirstOrDefault();

            if (user.IsNotNull())
            {
                chat.Users.Remove(user);
            }
            else
            {
                throw new NoUserException();
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromAllChats(Guid userId)
        {
            List<Guid> chats = await _chatRepo.GetChatsOfUser(userId);
            List<Task> removeTasks = new List<Task>();
            foreach (var chat in chats)
            {
                removeTasks.Add(this.RemoveUserFromChat(chat, userId));
            }

            await Task.WhenAll(removeTasks);
        }

        public async Task<Guid> GetChatCreator(Guid chatId)
        {
            var chat = await GetChatWithUsers(chatId);

            return chat.Users.Where(x => x.Role.Equals(Role.Creator)).Select(x => x.UserId).FirstOrDefault();
        }
    }
}
