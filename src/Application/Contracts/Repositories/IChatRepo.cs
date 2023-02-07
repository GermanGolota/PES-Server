using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Contracts.Repositories;

public interface IChatRepo
{
    /// <returns>Id of created chat</returns>
    Task<Guid> CreateChat(Guid admin, string chatName, string chatPassword, bool isMultiMessage);

    Task DeleteChat(Guid chatId);
    Task<ChatDisplayModel> GetChatModel(Guid chatId, Guid requesterId);
    Task<List<ChatInfoModel>> GetChats(ChatSelectionOptions options, Guid memberId);
    Task<List<ChatInfoModel>> GetMyChats(ChatSelectionOptions options, Guid memberId);
    Task<List<Guid>> GetChatsOfUser(Guid userId);
}