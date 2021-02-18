using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Core.Entities;
using MediatR;

namespace Application.CQRS.Commands
{
    public class RegisterChatHandler : IRequestHandler<RegisterChatCommand, bool>
    {
        private readonly IChatRepo _repo;

        public RegisterChatHandler(IChatRepo repo)
        {
            this._repo = repo;
        }
        public async Task<bool> Handle(RegisterChatCommand request, CancellationToken cancellationToken)
        {
            Guid chatId = Guid.NewGuid();
            Chat chat = new Chat
            {
                ChatId = chatId,
                ChatName = request.ChatName
            };
            User admin = new User
            {
                UserId = request.AdminId
            };

            await _repo.CreateChat(chat, admin);

            return true;
        }
    }
}
