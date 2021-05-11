using System;

namespace WebAPI.RequestModels
{
    public class EditMessageRequest
    {
        public Guid ChatId { get; set; }
        public string UpdatedMessage { get; set; }
        public Guid MessageId { get; set; }
    }
}
