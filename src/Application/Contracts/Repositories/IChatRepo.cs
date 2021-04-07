using Application.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IChatRepo
    {
        /// <returns>Id of created chat</returns>
        Task<Guid> CreateChat(Guid admin, string chatName, string chatPassword);
        Task DeleteChat(Guid chatId);
        Task<Chat> GetChatById(Guid chatId);
        Task<ChatDisplayModel> GetChatModel(Guid chatId);
        Task<List<ChatInfoModel>> GetChats(ChatSelectionOptions options);
        Task<List<Guid>> GetChatsOfUser(Guid userId);
    }
}
