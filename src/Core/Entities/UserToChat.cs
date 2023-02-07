using System;

namespace Core.Entities;

public class UserToChat
{
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
    public Role Role { get; set; }
}

public enum Role
{
    User = 1,
    Admin = 3,
    Creator = 5
}