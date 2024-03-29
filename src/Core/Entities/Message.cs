﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Message
{
    [Key] public Guid MessageId { get; set; }

    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }

    [MaxLength(2048)] public string Text { get; set; }

    public DateTime LastEditedDate { get; set; }
}