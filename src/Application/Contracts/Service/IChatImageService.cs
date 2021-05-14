using System;

namespace Application.Contracts.Service
{
    public interface IChatImageService
    {
        string GetRelativeImageLocation(Guid chatId);
    }
}
