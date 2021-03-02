using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebAPI.WebSockets
{
    public interface IWebSocketsManager
    {
        Guid AddSocket(WebSocket socket, Guid chatId);
        Task RemoveSocket(Guid socketId);
        Task RemoveSocketForPolicyVialtion(Guid socketId, string closureReason);
        Task SendTextMessageToSocketsConnectedToChat(Guid chatId, string textMessage);
    }
}