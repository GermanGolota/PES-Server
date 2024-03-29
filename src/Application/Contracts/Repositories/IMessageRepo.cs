﻿using System;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories;

public interface IMessageRepo
{
    Task AddMessageToChat(string message, Guid chatId, Guid userId);
    Task EditMessage(Guid messageId, string newText);
    Task DeleteMessage(Guid messageId);
}