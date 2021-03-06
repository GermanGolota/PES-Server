﻿using Application.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IChatRepo
    {
        Task CreateChat(Chat chat, User admin);
        Task DeleteChat(Guid chatId);
        Task<Chat> GetChatById(Guid chatId);
        Task<ChatDisplayModel> GetChatModel(Guid chatId);
        Task<List<ChatInfoModel>> GetChats(ChatSelectionOptions options);
    }
}
