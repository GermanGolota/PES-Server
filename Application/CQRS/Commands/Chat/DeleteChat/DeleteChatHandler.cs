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
                List<Guid> admins = await _repo.GetAdminsOfChat(request.ChatId);

                if (admins.Contains(request.UserId))
                {
                    await _repo.DeleteChat(request.ChatId);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
