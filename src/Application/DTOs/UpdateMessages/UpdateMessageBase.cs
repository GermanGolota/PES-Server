using Newtonsoft.Json;

namespace Application.DTOs.UpdateMessages
{
    public class UpdateMessageBase
    {
        [JsonProperty("typeAction")]
        public string ActionType { get; set; }
    }
}
