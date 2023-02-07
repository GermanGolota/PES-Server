using System.Collections.Generic;
using Core.Entities;

namespace Application.DTOs;

public class ChatDisplayModel
{
    public List<MessageModel> Messages { get; set; }
    public int MessagesCount { get; set; }
    public string ChatName { get; set; }
    public bool IsMultiMessage { get; set; }
    public string ChatImageLocation { get; set; }
    public Role? Role { get; set; }
}

public class PreChatDisplayModel
{
    public List<MessageModel> Messages { get; set; }
    public int MessagesCount { get; set; }
    public string ChatName { get; set; }
    public bool IsMultiMessage { get; set; }
    public List<UserToChat> Users { get; set; }
}