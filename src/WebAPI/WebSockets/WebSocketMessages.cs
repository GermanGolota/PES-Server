using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.WebSockets
{
    public class WebSocketMessages
    {
        public static string UnexpectedMessageType { get; } = "You are not supposed to send this message type";
    }
}
