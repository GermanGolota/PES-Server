using System;
using System.Collections.Generic;
using Core.Entities;

namespace Application.DTOs;

public class ChatInfoModel
{
    public Guid ChatId { get; set; }
    public string ChatName { get; set; }
    public int UserCount { get; set; }
    public Role? Role { get; set; }
    public string ChatImageLocation { get; set; }
}

public class PreChatInfoModel
{
    public Guid ChatId { get; set; }
    public string ChatName { get; set; }
    public int UserCount { get; set; }
    public List<UserToChat> Users { get; set; }
}