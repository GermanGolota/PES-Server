using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class ChatInfoModel
    {
        public Guid ChatId { get; set; }
        public string ChatName { get; set; }
        public int UserCount { get; set; }
    }
}
