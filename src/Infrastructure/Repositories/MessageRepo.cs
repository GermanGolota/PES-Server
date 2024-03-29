﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Core;
using Core.Entities;
using Core.Exceptions;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MessageRepo : IMessageRepo
{
    private readonly PESContext _context;

    public MessageRepo(PESContext context)
    {
        _context = context;
    }

    public async Task AddMessageToChat(string messageText, Guid chatId, Guid userId)
    {
        ChatMessageAdditionModel chat = await _context.Chats
            .AsNoTracking()
            .Include(x => x.Messages)
            .Where(x => x.ChatId.Equals(chatId))
            .Select(x => new ChatMessageAdditionModel
            {
                IsMultiMessage = x.IsMultiMessage,
                UserMessagesCount = x.Messages.Where(x => x.UserId.Equals(userId)).Count()
            })
            .FirstOrDefaultAsync();

        if (chat.IsMultiMessage || chat.UserMessagesCount == 0)
        {
            Message message = new()
            {
                ChatId = chatId,
                UserId = userId,
                Text = messageText,
                LastEditedDate = DateTime.UtcNow,
                MessageId = Guid.NewGuid()
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ExpectedException("Can not create second message in single message chat");
        }
    }

    public async Task DeleteMessage(Guid messageId)
    {
        Message message = await _context.Messages
            //this check can't be separated into a function, becouse it would
            //result in a client-side evaluation
            .Where(x => x.MessageId.Equals(messageId))
            .FirstOrDefaultAsync();

        if (message is null) throw new NoMessageException();

        _context.Messages.Remove(message);

        await _context.SaveChangesAsync();
    }

    public async Task EditMessage(Guid messageId, string newText)
    {
        Message message = _context.Messages
            .Where(x => x.MessageId.Equals(messageId))
            .FirstOrDefault();

        if (message.IsNotNull())
        {
            message.Text = newText;

            message.LastEditedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}