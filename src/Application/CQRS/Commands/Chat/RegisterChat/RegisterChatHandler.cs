using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
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

            string password = GetChatPassword(request);
            Guid chatId = await _repo.CreateChat(request.AdminId, request.ChatName, password, request.IsMultiMessage);

            response.Successfull = true;
            response.ResultMessage = $"Successfully registered chat {chatId}";
            return response;
        }

        private string GetChatPassword(RegisterChatCommand request)
        {
            string output = "1";
            if(!String.IsNullOrEmpty(request.ChatPassword))
            {
                output = request.ChatPassword;
            }
            return output;
        }
    }
}
