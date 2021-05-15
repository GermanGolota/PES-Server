using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Application.DTOs.Chat
{
    public class ChatImageUpdateRequest
    {
        public Stream ImageStream { get; set; }
        public string FileExtension { get; set; }
    }
}
