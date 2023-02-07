using System.Collections.Generic;

namespace Application.DTOs;

public class ChatsModel
{
    public List<ChatInfoModel> Chats { get; set; }
    public int ChatCount { get; set; }
}