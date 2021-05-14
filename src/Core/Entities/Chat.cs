using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    public class Chat
    {
        [Key]
        public Guid ChatId { get; set; }
        [MaxLength(150)]
        public string ChatName { get; set; }
        [MaxLength(50)]
        public string ChatPassword { get; set; }
        public bool IsMultiMessage { get; set; }
        public List<Message> Messages { get; set; }
        public List<UserToChat> Users { get; set; }
    }
}
