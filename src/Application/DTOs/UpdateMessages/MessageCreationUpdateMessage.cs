namespace Application.DTOs.UpdateMessages;

public class MessageCreationUpdateMessage : UpdateMessageBase
{
    public string Username { get; set; }
    public string Text { get; set; }
}