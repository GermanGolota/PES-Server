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
            Guid chatId = Guid.Empty;
            var result = await CommandRunner.Run(request, async(request) =>
            {
                string password = GetChatPassword(request);
                chatId = await _repo.CreateChat(request.AdminId, request.ChatName, password, request.IsMultiMessage);
            }, $"Successfully registered chat");
            if (result.Successfull)
            {
                result.ResultMessage += $" {chatId}";
            }

            return result;
        }

        private string GetChatPassword(RegisterChatCommand request)
        {
            string output = "1";
            if (!String.IsNullOrEmpty(request.ChatPassword))
            {
                output = request.ChatPassword;
            }
            return output;
        }
    }
}
