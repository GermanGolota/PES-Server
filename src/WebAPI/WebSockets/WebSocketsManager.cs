using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.WebSockets
{
    public class WebSocketsManager : IWebSocketsManager
    {
        private Dictionary<Guid, WebSocket> WebSockets { get; set; }
        public Guid AddSocket(WebSocket socket)
        {
            Guid guid = Guid.NewGuid();
            if (WebSockets.TryAdd(guid, socket))
            {
                return guid;
            }
            else
            {
                throw new Exception("Can't add a socket");
            }
        }
        public async Task RemoveSocket(Guid socketId)
        {

            if (WebSockets.ContainsKey(socketId))
            {
                var webSocket = WebSockets[socketId];

                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Connection closed as requested", CancellationToken.None);

                WebSockets.Remove(socketId);
            }
        }
    }
}
