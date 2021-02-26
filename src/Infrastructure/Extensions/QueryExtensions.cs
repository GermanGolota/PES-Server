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
        public static IEnumerable<ChatInfoModel> MapChatsToInfoModels(this IEnumerable<Chat> chats)
        {
            return chats.Select(chat => new ChatInfoModel
            {
                ChatId = chat.ChatId,
                ChatName = chat.ChatName,
                UserCount = chat.Users.Count
            });
        }
        public static IQueryable<ChatInfoModel> MapChatsToInfoModels(this IQueryable<Chat> chats)
        {
            return chats.Select(chat => new ChatInfoModel
            {
                ChatId = chat.ChatId,
                ChatName = chat.ChatName,
                UserCount = chat.Users.Count
            });
        }
    }
}
