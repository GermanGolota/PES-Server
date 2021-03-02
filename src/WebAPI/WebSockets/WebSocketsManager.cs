using System;
using System.Collections.Generic;
using System.Net.WebSockets;
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
                    "Connection closed as requested", CancellationToken.None);

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
    }
}
