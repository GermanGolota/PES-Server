using System;

namespace WebAPI.RequestModels
{
    public class EditMessageRequest
    {
        public Guid ChatId { get; set; }
        public string UpdatedMessage { get; set; }
    }
}
