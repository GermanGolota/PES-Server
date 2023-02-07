using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Queries;

public class GetChatsHandler : IRequestHandler<GetChatsQuery, ChatsModel>
{
    private readonly IChatRepo _repo;

    public GetChatsHandler(IChatRepo repo)
    {
        _repo = repo;
    }

    public async Task<ChatsModel> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        List<ChatInfoModel> chats = await _repo.GetChats(request.Options, request.UserId);

        ChatsModel chatsModel = new()
        {
            Chats = chats,
            ChatCount = chats.Count
        };

        return chatsModel;
    }
}