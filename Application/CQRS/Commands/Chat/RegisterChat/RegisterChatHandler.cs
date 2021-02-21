using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Core.Entities;
using MediatR;

namespace Application.CQRS.Commands
{
    public class RegisterChatHandler : IRequestHandler<RegisterChatCommand, CommandResponse>
    {
        private readonly IChatRepo _repo;

        public RegisterChatHandler(IChatRepo repo)
        {
            this._repo = repo;
        }
        public async Task<CommandResponse> Handle(RegisterChatCommand request, 
            CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

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

            response.Successfull = true;
            response.ResultMessage = $"Successfully registered chat {chatId}";
            return response;
        }
    }
}
