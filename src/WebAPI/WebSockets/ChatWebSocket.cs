using System;
using System.Net.WebSockets;

namespace WebAPI.WebSockets
{
    public class ChatWebSocket
    {
        public Guid ChatId { get; set; }
        public WebSocket WebSocket{ get; set; }
    }
}
