using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Service;
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
            _repo = repo;
        }
        public async Task<ChatDisplayModel> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            var chatId = new Guid(request.ChatId);
            var chat = await _repo.GetChatModel(chatId, request.RequesterId);
            return chat;
        }
    }
}
