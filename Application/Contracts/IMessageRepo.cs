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
        Task EditMessage(Guid userId, Guid chatId, string newText);
        Task<Guid> FindUserMessageInChat(Guid userId, Guid chatId);
    }
}
