using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Service;
using Application.DTOs;
using Core;
using Core.Entities;
using Core.Exceptions;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatRepo : IChatRepo
{
    private readonly IChatImageService _chatImage;
    private readonly PESContext _context;

    public ChatRepo(PESContext context, IChatImageService chatImage)
    {
        _context = context;
        _chatImage = chatImage;
    }

    public async Task<Guid> CreateChat(Guid adminId, string chatName, string chatPassword, bool isMultiMessage)
    {
        Guid chatId = Guid.NewGuid();
        UserToChat admin = new()
        {
            ChatId = chatId,
            UserId = adminId,
            Role = Role.Creator
        };
        Chat chat = new()
        {
            ChatName = chatName,
            ChatPassword = chatPassword,
            IsMultiMessage = isMultiMessage,
            Users = new List<UserToChat>
            {
                admin
            }
        };

        _context.Chats.Add(chat);

        await _context.SaveChangesAsync();

        return chatId;
    }

    public async Task DeleteChat(Guid chatId)
    {
        Chat chat = await _context.Chats.FindAsync(chatId);

        if (chat is null) throw new NoChatException();

        _context.Chats.Remove(chat);

        await _context.SaveChangesAsync();
    }

    public async Task<ChatDisplayModel> GetChatModel(Guid chatId, Guid requesterId)
    {
        PreChatDisplayModel chat = await _context.Chats
            .AsNoTracking()
            .Include(x => x.Messages)
            .ThenInclude(x => x.User)
            .Where(x => x.ChatId.Equals(chatId))
            .Select(x => new PreChatDisplayModel
            {
                ChatName = x.ChatName,
                IsMultiMessage = x.IsMultiMessage,
                MessagesCount = x.Messages.Count,
                Messages = (List<MessageModel>)x.Messages
                    .OrderByDescending(message => message.LastEditedDate)
                    .Select(message => new MessageModel
                    {
                        Message = message.Text,
                        Username = message.User.Username,
                        MessageId = message.MessageId,
                        IsMine = message.UserId == requesterId
                    }),
                Users = x.Users
            })
            .FirstOrDefaultAsync();

        if (chat is null) throw new NoChatException();

        ChatDisplayModel output = new ChatDisplayModel
        {
            ChatImageLocation = _chatImage.GetRelativeImageLocation(chatId),
            ChatName = chat.ChatName,
            IsMultiMessage = chat.IsMultiMessage,
            Messages = chat.Messages,
            MessagesCount = chat.MessagesCount,
            Role = chat.Users.Where(x => x.UserId.Equals(requesterId)).FirstOrDefault()?.Role
        };

        return output;
    }

    public async Task<List<ChatInfoModel>> GetChats(ChatSelectionOptions options, Guid memberId)
    {
        IQueryable<Chat> query = _context.Chats
            .AsNoTracking()
            .Include(x => x.Users)
            .AsQueryable();

        return await GetSelectionForUserFromQuery(options, memberId, query);
    }

    public async Task<List<ChatInfoModel>> GetMyChats(ChatSelectionOptions options, Guid memberId)
    {
        IQueryable<Chat> query = _context.Chats
            .AsNoTracking()
            .Include(x => x.Users)
            .Where(x => x.Users.Where(x => x.UserId.Equals(memberId)).Count() > 0);

        return await GetSelectionForUserFromQuery(options, memberId, query);
    }


    public async Task<List<Guid>> GetChatsOfUser(Guid userId)
    {
        return await _context.Chats
            .Include(x => x.Users)
            .ContainsUser(userId)
            .Select(x => x.ChatId)
            .ToListAsync();
    }

    private async Task<List<ChatInfoModel>> GetSelectionForUserFromQuery(ChatSelectionOptions options,
        Guid memberId, IQueryable<Chat> query)
    {
        query = ApplySearchOptionsToQuery(options, query);

        IQueryable<PreChatInfoModel> result = query.MapChatsToInfoModels();

        List<PreChatInfoModel> pre = await result.ToListAsync();
        return ProcessChatModel(memberId, pre);
    }

    private List<ChatInfoModel> ProcessChatModel(Guid memberId, List<PreChatInfoModel> pre)
    {
        return pre.Select(x => new ChatInfoModel
        {
            ChatId = x.ChatId,
            ChatName = x.ChatName,
            UserCount = x.Users.Count,
            Role = x.Users.Where(x => x.UserId.Equals(memberId)).FirstOrDefault()?.Role,
            ChatImageLocation = _chatImage.GetRelativeImageLocation(x.ChatId)
        }).ToList();
    }

    private IQueryable<Chat> ApplySearchOptionsToQuery(ChatSelectionOptions options, IQueryable<Chat> query)
    {
        int count = options.ChatsPerPage;
        bool takeAll = count.Equals(-1);

        string term = options.SearchTerm;
        bool takeAny = string.IsNullOrEmpty(term);

        if (!takeAll)
        {
            int numberOfPrecidingPages = options.PageNumber - 1;
            int skipCount = options.ChatsPerPage * numberOfPrecidingPages;

            query = query.OrderBy(x => x.ChatName);
            query = query.Skip(skipCount);
            query = query.Take(count);
        }

        if (!takeAny) query = query.Where(x => EF.Functions.Like(x.ChatName, $"%{term}%"));

        if (options.MultiMessage != ChatMultiMessageMode.Any)
        {
            bool isMulti = ChatMultiMessageMode.MultiMessage == options.MultiMessage;
            query = query.Where(x => x.IsMultiMessage == isMulti);
        }

        return query;
    }
}