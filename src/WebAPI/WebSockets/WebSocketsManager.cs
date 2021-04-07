using Application.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using Core.Extensions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.WebSockets
{
    public class WebSocketsManager : IWebSocketsManager
    {
        private ConcurrentDictionary<Guid, ChatWebSocket> WebSockets { get; set; }
        
        public Guid AddSocket(WebSocket socket, Guid chatId)
        {
            Guid guid = Guid.NewGuid();
            ChatWebSocket chatSocket = new ChatWebSocket
            {
                Client = socket,
                ChatId = chatId
            };
            WebSockets.TryAddWithRetries(guid, chatSocket, 10);
            return guid;
        }
        public async Task RemoveSocket(Guid socketId)
        {
            if (WebSockets.ContainsKey(socketId))
            {
                var webSocket = WebSockets[socketId];
                if (WebSocketIsOpen(webSocket))
                {
                    await webSocket.Client.CloseAsync(WebSocketCloseStatus.NormalClosure,
                        WebSocketMessages.ConnectionClosingRequestSucceeded,
                        CancellationToken.None);
                }

                WebSockets.TryRemoveWithRetries(socketId, 10);
            }
        }

        public async Task RemoveSocketForPolicyVialtion(Guid socketId, string closureReason)
        {
            if (WebSockets.ContainsKey(socketId))
            {
                var webSocket = WebSockets[socketId];
                if (WebSocketIsOpen(webSocket))
                {
                    await webSocket.Client.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                    closureReason, CancellationToken.None);
                }
                WebSockets.TryRemoveWithRetries(socketId, 10);
            }
        }

        private static bool WebSocketIsOpen(ChatWebSocket webSocket)
        {
            return webSocket.Client.State.Equals(WebSocketState.CloseReceived)
                                || webSocket.Client.State.Equals(WebSocketState.Open);
        }

        public async Task SendTextMessageToSocketsConnectedToChat(Guid chatId, string textMessage)
        {
            var webSockets = WebSockets.Values.Where(x => x.ChatId.Equals(chatId)).ToList();

            ArraySegment<byte> message = CreateMessageFrom(textMessage);
            List<Task> sendTasks = new List<Task>();
            foreach (var webSocket in webSockets)
            {
                sendTasks.Add(SendMessage(message, webSocket.Client));
            }

            await Task.WhenAll(sendTasks);
        }

        private ArraySegment<byte> CreateMessageFrom(string textMessage)
        {
            var buffer = Encoding.ASCII.GetBytes(textMessage);
            ArraySegment<byte> message = new(buffer);
            return message;
        }

        public Task SendMessage(ArraySegment<byte> message, WebSocket socket)
        {
            return socket.SendAsync(message, WebSocketMessageType.Text,
                   endOfMessage: true, cancellationToken: CancellationToken.None);
        }

        public async Task SendJsonToSocketsConnectedToChat<T>(Guid chatId, T jsonObject)
        {
            string message = JsonConvert.SerializeObject(jsonObject);
            await SendTextMessageToSocketsConnectedToChat(chatId, message);
        }
    }
}
