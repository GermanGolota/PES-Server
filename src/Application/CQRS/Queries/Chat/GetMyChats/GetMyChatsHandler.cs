using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Queries;

public class GetMyChatsHandler : IRequestHandler<GetMyChatsQuery, ChatsModel>
{
    private readonly IChatRepo _repo;

    public GetMyChatsHandler(IChatRepo repo)
    {
        _repo = repo;
    }

    public async Task<ChatsModel> Handle(GetMyChatsQuery request, CancellationToken cancellationToken)
    {
        List<ChatInfoModel> chats = await _repo.GetMyChats(request.Options, request.UserId);

        ChatsModel chatsModel = new()
        {
            Chats = chats,
            ChatCount = chats.Count
        };

        return chatsModel;
    }
}