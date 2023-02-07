using System;

namespace Application.DTOs.Chat;

public class ChatMemberModelAdmin : ChatMemberModel
{
    public Guid MemberId { get; set; }
}