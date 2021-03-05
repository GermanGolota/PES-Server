using Application.DTOs.UpdateMessages;
using System;
using System.Threading.Tasks;


namespace Application.Contracts
{
    public interface IMessageSender
    {
        Task SendMessageToSocket(UpdateMessageBase message, Guid chatId);
    }
}
