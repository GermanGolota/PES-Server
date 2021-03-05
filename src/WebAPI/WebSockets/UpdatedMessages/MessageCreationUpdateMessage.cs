using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.WebSockets.UpdatedMessages
{
    public class MessageCreationUpdateMessage : UpdateMessageBase
    {
        public string Username { get; set; }
        public string Text { get; set; }
    }
}
