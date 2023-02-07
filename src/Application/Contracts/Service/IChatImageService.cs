using System;
using System.Threading.Tasks;
using Application.DTOs.Chat;

namespace Application.Contracts.Service;

public interface IChatImageService
{
    string GetRelativeImageLocation(Guid chatId);
    Task UpdateChatsImage(Guid chatId, ChatImageUpdateRequest request);
}