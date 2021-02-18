using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class ChatsModel
    {
        public List<ChatInfoModel> Chats { get; set; }
        public int ChatCount { get; set; }
    }
}
