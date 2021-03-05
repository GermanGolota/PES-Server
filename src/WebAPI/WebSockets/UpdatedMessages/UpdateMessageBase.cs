using Newtonsoft.Json;

namespace WebAPI.WebSockets.UpdatedMessages
{
    public class UpdateMessageBase
    {
        [JsonProperty("typeAction")]
        public string ActionType { get; set; }
    }
}
