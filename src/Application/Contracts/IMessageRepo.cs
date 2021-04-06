using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IMessageRepo
    {
        Task AddMessageToChat(string message, Guid chatId, Guid userId);
        Task EditMessage(Guid userId, Guid chatId, string newText);
        Task DeleteMessage(Guid userId, Guid chatId);
        Task<List<string>> GetAllUserMessages(Guid userId);
        Task<Guid> FindUserMessageInChat(Guid userId, Guid chatId);
    }
}
