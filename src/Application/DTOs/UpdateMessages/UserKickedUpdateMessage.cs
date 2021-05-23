using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.UpdateMessages
{
    public class UserKickedUpdateMessage : UpdateMessageBase
    {
        public string Username { get; set; }
    }
}
