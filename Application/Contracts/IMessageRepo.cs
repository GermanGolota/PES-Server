using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IMessageRepo
    {
        Task AddMessageToChat(Message message);
        Task AddMessageToChat(string message, Guid chatId, Guid userId);
        Task EditMessage(Guid userId, Guid chatId, string newText);
        Task<Guid> FindUserMessageInChat(Guid userId, Guid chatId);
    }
}
