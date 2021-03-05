using Application.Contracts;
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
    public class DeleteMessageHandler : IRequestHandler<DeleteMessageCommand, CommandResponse>
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
        public async Task<CommandResponse> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response;
            try
            {
                await _repo.DeleteMessage(request.UserId, request.ChatId);
                response = CommandResponse.CreateSuccessfull("Successfully deleted message");
                await SendUpdateMessage(request);
            }
            catch (ExpectedException exc)
            {
                response = CommandResponse.CreateUnsuccessfull(exc.Message);
            }
            catch
            {
                response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.ServerError);
            }
            return response;
        }

        private async Task SendUpdateMessage(DeleteMessageCommand request)
        {
            string username = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateMessageDeletedUpdate(username);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
