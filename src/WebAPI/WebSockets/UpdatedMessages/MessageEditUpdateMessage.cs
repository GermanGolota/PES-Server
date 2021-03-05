namespace WebAPI.WebSockets.UpdatedMessages
{
    public class MessageEditUpdateMessage : UpdateMessageBase
    {
        public string Username { get; set; }
        public string NewText { get; set; }
    }
}
