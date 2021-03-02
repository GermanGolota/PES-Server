using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebAPI.WebSockets
{
    public interface IWebSocketsManager
    {
        Guid AddSocket(WebSocket socket);
        Task RemoveSocket(Guid socketId);
    }
}