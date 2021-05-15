using System;

namespace Application.DTOs
{
    public class MessageModel
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public Guid MessageId { get; set; }
        public bool IsMine { get; set; }
    }
}