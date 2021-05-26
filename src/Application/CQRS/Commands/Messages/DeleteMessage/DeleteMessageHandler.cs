using Application.Contracts;
using Application.Contracts.Repositories;
using Application.DTOs;
using Application.DTOs.UpdateMessages;
using Core.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Commands
{
    public class DeleteMessageHandler : PesCommand<DeleteMessageCommand>
    {
        private readonly IMessageRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly IMessageSender _sender;

        public DeleteMessageHandler(IMessageRepo repo, IUserRepo userRepo, IMessageSender sender)
        {
            this._repo = repo;
            _userRepo = userRepo;
            _sender = sender;
        }

        public override string SuccessMessage => "Successfully deleted message";

        public override async Task Run(DeleteMessageCommand request, CancellationToken token)
        {
            await _repo.DeleteMessage(request.MessageId);
            await SendUpdateMessage(request);
        }

        private async Task SendUpdateMessage(DeleteMessageCommand request)
        {
            string username = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateMessageDeletedUpdate(username);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
