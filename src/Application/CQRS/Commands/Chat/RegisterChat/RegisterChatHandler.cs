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
    public class RegisterChatHandler : PesCommand<RegisterChatCommand>
    {
        private readonly IChatRepo _repo;

        public RegisterChatHandler(IChatRepo repo)
        {
            this._repo = repo;
        }

        private string _successMessage;
        public override string SuccessMessage => _successMessage;

        public override async Task Run(RegisterChatCommand request, CancellationToken token)
        {
            string password = GetChatPassword(request);
            var chatId = await _repo.CreateChat(request.AdminId, request.ChatName, password, request.IsMultiMessage);
            _successMessage = $"Successfully registered chat {chatId}";
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
