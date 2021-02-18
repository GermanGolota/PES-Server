using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
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
            var chat = await _repo.GetChatById(new Guid(request.ChatId));
            ChatDisplayModel output = new ChatDisplayModel
            {
                ChatName = chat.ChatName
            };
        }
    }
}
