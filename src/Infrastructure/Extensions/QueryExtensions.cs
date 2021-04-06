using Application.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<ChatInfoModel> MapChatsToInfoModels(this IQueryable<Chat> chats)
        {
            return chats.Select(chat => new ChatInfoModel
            {
                ChatId = chat.ChatId,
                ChatName = chat.ChatName,
                UserCount = chat.Users.Count + chat.Admins.Count
            });
        }
        public static IQueryable<Chat> ContainsUser(this IQueryable<Chat> chats, Guid userId)
        {
            return chats.Where(x => x.Admins.Where(admin => admin.UserId.Equals(userId)).Any()
                                 || x.Users.Where(user => user.UserId.Equals(userId)).Any());
        }
    }
}
