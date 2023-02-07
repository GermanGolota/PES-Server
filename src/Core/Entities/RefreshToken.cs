using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class RefreshToken
{
    [Key] public Guid RefreshTokenId { get; set; }

    public Guid UserId { get; set; }
    public string Token { get; set; }
}