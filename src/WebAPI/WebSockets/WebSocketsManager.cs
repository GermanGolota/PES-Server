﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.WebSockets
{
    public class WebSocketsManager : IWebSocketsManager
    {
        private Dictionary<Guid, ChatWebSocket> WebSockets { get; set; }
        
        public Guid AddSocket(WebSocket socket, Guid chatId)
        {
            Guid guid = Guid.NewGuid();
            ChatWebSocket chatSocket = new ChatWebSocket
            {
                WebSocket = socket,
                ChatId = chatId
            };
            WebSockets.Add(guid, chatSocket);
            return guid;
        }
        public async Task RemoveSocket(Guid socketId)
        {
            if (WebSockets.ContainsKey(socketId))
            {
                var webSocket = WebSockets[socketId];

                await webSocket.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                    WebSocketMessages.ConnectionClosingRequestSucceeded,
                    CancellationToken.None);

                WebSockets.Remove(socketId);
            }
        }

        public async Task RemoveSocketForPolicyVialtion(Guid socketId, string closureReason)
        {
            if (WebSockets.ContainsKey(socketId))
            {
                var webSocket = WebSockets[socketId];

                await webSocket.WebSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                    closureReason, CancellationToken.None);

                WebSockets.Remove(socketId);
            }
        }

        public async Task SendTextMessageToSocketsConnectedToChat(Guid chatId, string textMessage)
        {
            var webSockets = WebSockets.Values.Where(x => x.ChatId.Equals(chatId)).ToList();

            ArraySegment<byte> message = CreateMessageFrom(textMessage);
            List<Task> sendTasks = new List<Task>();
            foreach (var webSocket in webSockets)
            {
                sendTasks.Add(SendMessage(message, webSocket.WebSocket));
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
    }
}
