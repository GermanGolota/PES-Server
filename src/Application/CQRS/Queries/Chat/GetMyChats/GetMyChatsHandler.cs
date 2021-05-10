using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Queries
{
    public class GetMyChatsHandler : IRequestHandler<GetMyChatsQuery, ChatsModel>
    {
        private readonly IChatRepo _repo;

        public GetMyChatsHandler(IChatRepo repo)
        {
            _repo = repo;
        }

        public async Task<ChatsModel> Handle(GetMyChatsQuery request, CancellationToken cancellationToken)
        {
            var chats = await _repo.GetMyChats(request.Options, request.UserId);

            ChatsModel chatsModel = new ChatsModel
            {
                Chats = chats,
                ChatCount = chats.Count
            };

            return chatsModel;
        }
    }
}
