using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Queries
{
    public class GetChatsHandler : IRequestHandler<GetChatsQuery, ChatsModel>
    {
        private readonly IChatRepo _repo;

        public GetChatsHandler(IChatRepo repo)
        {
            this._repo = repo;
        }
        public async Task<ChatsModel> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            var chats = await _repo.GetChats(request.Options);

            ChatsModel chatsModel = new ChatsModel
            {
                Chats = chats,
                ChatCount = chats.Count
            };

            return chatsModel;
        }
    }
}
