using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    public class Message
    {
        [Key]
        public Guid MessageId { get; set; }
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        [MaxLength(2048)]
        public string Text { get; set; }
    }
}
