using System;
using System.Threading.Tasks;
using Application.DTOs.UpdateMessages;

namespace Application.Contracts;

public interface IMessageSender
{
    Task SendMessageToSocket(UpdateMessageBase message, Guid chatId);
}