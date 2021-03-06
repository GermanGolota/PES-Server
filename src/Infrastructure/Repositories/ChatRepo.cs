using Application.Contracts;
using Application.DTOs;
using Application.DTOs.Chat;
using Core;
using Core.Entities;
using Core.Exceptions;
using Core.Extensions;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ChatRepo : IChatRepo, IChatMembersService
    {
        private readonly PESContext _context;

        public ChatRepo(PESContext context)
        {
            this._context = context;
        }

        public async Task AddUser(Guid chatId, Guid userId, string chatPassword)
        {
            var chat = await GetChatWithUsers(chatId);

            if(UserAlreadyPresent(userId, chat.Users))
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
                .Include(x=>x.Admins)
                .Where(x => x.ChatId.Equals(chatId))
                .FirstOrDefaultAsync();

            if(chat is null)
            {
                throw new NoChatException();
            }

            return chat;
        }
        public async Task PromoteToAdmin(Guid chatId, Guid userId)
        {
            var chat = await GetChatWithUsers(chatId);

            if(UserAlreadyPresent(userId, chat.Users))
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
                if(UserIsAdmin(userId, chat.Admins))
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

        public async Task<List<Guid>> GetAdminsOfChat(Guid chatId)
        {
            var chat = await _context.Chats.Where(x => x.ChatId.Equals(chatId))
                .Include(x=>x.Admins)
                .FirstOrDefaultAsync();

            if(chat is null)
            {
                throw new NoChatException();
            }

            return chat.Admins.Select(x => x.UserId).ToList();
        }

        public async Task<Chat> GetChatById(Guid chatId)
        {
            return await _context.Chats.FindAsync(chatId);
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

            var query = _context.Chats.Include(x => x.Users);

            if(!takeAll)
            {
                int numberOfPrecidingPages = options.PageNumber - 1;
                int skipCount = options.ChatsPerPage * numberOfPrecidingPages;

                query.Skip(skipCount);

                query.Take(count);
            }

            if(!takeAny)
            {
                query.Where(x => EF.Functions.Like(x.ChatName, $"%{term}%"));
            }

            var result = query.MapChatsToInfoModels();

            return await result.ToListAsync();
        }

        public async Task<MemberModel> GetMember(Guid chatId, Guid userId)
        {
            var chat = await GetChatWithUsers(chatId);

            var user = chat.Users.Where(x => x.UserId.Equals(chatId)).FirstOrDefault();

            var admin = chat.Admins.Where(x => x.UserId.Equals(chatId)).FirstOrDefault();

            bool userIsAdmin = admin.IsNotNull();
            bool userIsInChat = user.IsNotNull();

            if(UserNotInChat(userIsAdmin, userIsInChat))
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

        public async Task<List<ChatMemberModel>> GetChatMembers(Guid chatId)
        {
            var members =  await GetMembers(chatId);
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

        public async Task RemoveUserFromChat(Guid chatId, Guid userId)
        {
            var chat = await GetChatWithUsers(userId);

            var userQuerry = chat.Users.Where(x => x.UserId.Equals(userId));
            bool userInChat = userQuerry.Any();

            var adminQuerry = chat.Admins.Where(x => x.UserId.Equals(userId));
            bool userIsAdmin = adminQuerry.Any();

            if(userInChat)
            {
                var user = userQuerry.FirstOrDefault();
                chat.Users.Remove(user);
            }
            else
            {
                if(userIsAdmin)
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
    }
}
