using Application.DTOs.Chat;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Application.Contracts.Service
{
    public interface IChatImageService
    {
        string GetRelativeImageLocation(Guid chatId);
        Task UpdateChatsImage(Guid chatId, ChatImageUpdateRequest request);
    }
}
