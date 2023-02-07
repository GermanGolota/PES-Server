using System;

namespace WebAPI.RequestModels;

public class PostMessageRequest
{
    public Guid ChatId { get; set; }
    public string Message { get; set; }
}