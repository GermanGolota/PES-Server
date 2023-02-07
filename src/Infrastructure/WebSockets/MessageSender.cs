using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs.UpdateMessages;

namespace Infrastructure.WebSockets;

public class MessageSender : IMessageSender
{
    private readonly IWebSocketsManager _webSocketsManager;

    public MessageSender(IWebSocketsManager webSocketsManager)
    {
        _webSocketsManager = webSocketsManager;
    }

    public Task SendMessageToSocket(UpdateMessageBase message, Guid chatId)
    {
        Thread separateThread = new Thread(async () => await SendUpdateMessage(chatId, message));
        separateThread.Priority = ThreadPriority.Normal;
        separateThread.Start();
        return Task.CompletedTask;
    }

    private async Task SendUpdateMessage(Guid chatId, UpdateMessageBase updateMessage)
    {
        await _webSocketsManager.SendJsonToSocketsConnectedToChat(chatId, updateMessage);
    }
}