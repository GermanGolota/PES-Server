using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using Core.Extensions;
using MediatR;

namespace Application.CQRS.Queries.Chat.GetChat
{
    public class GetChatHandler : IRequestHandler<GetChatQuery, ChatDisplayModel>
    {
        private readonly IChatRepo _repo;

        public GetChatHandler(IChatRepo repo)
        {
            this._repo = repo;
        }
        public async Task<ChatDisplayModel> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            var chat = await _repo.GetChatModel(new Guid(request.ChatId), request.RequesterId);
            return chat;
        }
    }
}
