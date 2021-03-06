using Application.Contracts;
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

            if (UserAlreadyPresent(userId, chat.Users))
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
                UserId = userId
            };

            chat.Users.Add(user);

            await _context.SaveChangesAsync();
        }
        private bool UserAlreadyPresent(Guid userId, List<UserToChat> users)
        {
            return users.IsNotNull() && users.Where(x => x.UserId.Equals(userId)).Any();
        }
        private async Task<Chat> GetChatWithUsers(Guid chatId)
        {
            var chat = await _context.Chats
                .Include(x => x.Users)
                .Include(x => x.Admins)
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
                .Include(x => x.Admins)
                .FirstOrDefaultAsync();

            if (chat is null)
            {
                throw new NoChatException();
            }

            return chat.Admins.Select(x => x.UserId).ToList();
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
            return chat.Users.Select(x => x.UserId).Union(chat.Admins.Select(x => x.UserId)).ToList();
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

            var user = chat.Users.Where(x => x.UserId.Equals(chatId)).FirstOrDefault();

            var admin = chat.Admins.Where(x => x.UserId.Equals(chatId)).FirstOrDefault();

            bool userIsAdmin = admin.IsNotNull();
            bool userIsInChat = user.IsNotNull();

            if (UserNotInChat(userIsAdmin, userIsInChat))
            {
                throw new NoUserException();
            }

            string username = _context.Users
                .Where(x => x.UserId.Equals(userId))
                .FirstOrDefault()
                ?.Username;

            MemberModel member = new MemberModel
            {
                IsAdmin = userIsAdmin,
                Username = username
            };

            return member;
        }

        private bool UserNotInChat(bool userIsAdmin, bool userIsInChat)
        {
            return !userIsAdmin && !userIsInChat;
        }

        public async Task PromoteToAdmin(Guid chatId, Guid userId)
        {
            var chat = await GetChatWithUsers(chatId);

            if (UserAlreadyPresent(userId, chat.Users))
            {
                AdminToChat admin = new AdminToChat
                {
                    ChatId = chatId,
                    UserId = userId
                };

                chat.Admins.Add(admin);

                var user = chat.Users.Where(x => x.UserId.Equals(userId)).FirstOrDefault();
                chat.Users.Remove(user);

                await _context.SaveChangesAsync();
            }
            else
            {
                if (UserIsAdmin(userId, chat.Admins))
                {
                    throw new CannotPromoteAdminException();
                }
                throw new NoUserException();
            }
        }

        private bool UserIsAdmin(Guid userId, List<AdminToChat> admins)
        {
            return admins.Where(x => x.UserId.Equals(userId)).Any();
        }

        public async Task RemoveUserFromChat(Guid chatId, Guid userId)
        {
            var chat = await GetChatWithUsers(userId);

            var userQuerry = chat.Users.Where(x => x.UserId.Equals(userId));
            bool userInChat = userQuerry.Any();

            var adminQuerry = chat.Admins.Where(x => x.UserId.Equals(userId));
            bool userIsAdmin = adminQuerry.Any();

            if (userInChat)
            {
                var user = userQuerry.FirstOrDefault();
                chat.Users.Remove(user);
            }
            else
            {
                if (userIsAdmin)
                {
                    var admin = adminQuerry.FirstOrDefault();
                    chat.Admins.Remove(admin);
                }
                else
                {
                    throw new NoUserException();
                }
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
    }
}
