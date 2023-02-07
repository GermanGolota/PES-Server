using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Queries.Chat.GetChat;

public class GetChatHandler : IRequestHandler<GetChatQuery, ChatDisplayModel>
{
    private readonly IChatRepo _repo;

    public GetChatHandler(IChatRepo repo)
    {
        _repo = repo;
    }

    public async Task<ChatDisplayModel> Handle(GetChatQuery request, CancellationToken cancellationToken)
    {
        Guid chatId = new Guid(request.ChatId);
        ChatDisplayModel chat = await _repo.GetChatModel(chatId, request.RequesterId);
        return chat;
    }
}