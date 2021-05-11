using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.DTOs;
using Application.DTOs.UpdateMessages;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class EditMessageHandler : IRequestHandler<EditMessageCommand, CommandResponse>
    {
        private readonly IMessageRepo _repo;
        private readonly IMessageSender _sender;
        private readonly IUserRepo _userRepo;

        public EditMessageHandler(IMessageRepo repo, IMessageSender sender, IUserRepo userRepo)
        {
            this._repo = repo;
            _sender = sender;
            _userRepo = userRepo;
        }
        public async Task<CommandResponse> Handle(EditMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repo.EditMessage(request.MessageId, request.UpdatedMessage);
            }
            catch(ExpectedException exc)
            {
                return new CommandResponse
                {
                    Successfull = false,
                    ResultMessage = exc.Message
                };
            }
            catch
            {
                return new CommandResponse
                {
                    Successfull = false,
                    ResultMessage = "Something went wrong"
                };
            }

            var response = new CommandResponse
            {
                Successfull = true,
                ResultMessage = "Successfullt edited"
            };

            await SendUpdateMessage(request);

            return response;
        }

        private async Task SendUpdateMessage(EditMessageCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateMessageEditedUpdate(userName, request.UpdatedMessage);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
