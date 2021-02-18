using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using MediatR;

namespace Application.CQRS.Commands
{
    public class DeleteChatHandler : IRequestHandler<DeleteChatCommand, bool>
    {
        private readonly IChatRepo _repo;

        public DeleteChatHandler(IChatRepo repo)
        {
            this._repo = repo;
        }
        public async Task<bool> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repo.DeleteChat(request.ChatId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
