using System.IO;

namespace Application.DTOs.Chat;

public class ChatImageUpdateRequest
{
    public Stream ImageStream { get; set; }
    public string FileExtension { get; set; }
}