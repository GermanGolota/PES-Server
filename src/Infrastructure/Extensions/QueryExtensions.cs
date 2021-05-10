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
        public static IQueryable<PreChatInfoModel> MapChatsToInfoModels(this IQueryable<Chat> chats, Guid memberId)
        {
            return chats.Select(chat => new PreChatInfoModel
            {
                ChatId = chat.ChatId,
                ChatName = chat.ChatName,
                UserCount = chat.Users.Count,
                Users = chat.Users
            });
        }
        public static IQueryable<Chat> ContainsUser(this IQueryable<Chat> chats, Guid userId)
        {
            return chats.Where(x => x.Users.Any(user => user.UserId.Equals(userId)));
        }
    }
}
