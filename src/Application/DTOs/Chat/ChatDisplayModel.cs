using System.Collections.Generic;

namespace Application.DTOs
{
    public class ChatDisplayModel
    {
        public List<MessageModel> Messages{ get; set; }
        public string ChatName { get; set; }
        public bool IsMultiMessage { get; set; }
    }
}
