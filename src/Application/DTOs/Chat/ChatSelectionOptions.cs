namespace Application.DTOs;

public class ChatSelectionOptions
{
    public int ChatsPerPage { get; set; }
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; }
    public ChatMultiMessageMode MultiMessage { get; set; }
}

public enum ChatMultiMessageMode
{
    Any,
    MultiMessage,
    SingleMessage
}