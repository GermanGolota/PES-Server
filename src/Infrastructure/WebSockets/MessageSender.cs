using Application.Contracts;
using Application.DTOs.UpdateMessages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.WebSockets
{
    public class MessageSender : IMessageSender
    {
        private readonly IWebSocketsManager _webSocketsManager;

        public MessageSender(IWebSocketsManager webSocketsManager)
        {
            _webSocketsManager = webSocketsManager;
        }
        public Task SendMessageToSocket(UpdateMessageBase message, Guid chatId)
        {
            var separateThread = new Thread(async () => await SendUpdateMessage(chatId, message));
            separateThread.Priority = ThreadPriority.Normal;
            separateThread.Start();
            return Task.CompletedTask;
        }

        private async Task SendUpdateMessage(Guid chatId, UpdateMessageBase updateMessage)
        {
            await _webSocketsManager.SendJsonToSocketsConnectedToChat(chatId, updateMessage);
        }
    }
}
