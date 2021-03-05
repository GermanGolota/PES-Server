using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Application.DTOs.UpdateMessages;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class DeleteChatHandler : IRequestHandler<DeleteChatCommand, CommandResponse>
    {
        private readonly IChatRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly IMessageSender _sender;

        public DeleteChatHandler(IChatRepo repo, IUserRepo userRepo, IMessageSender sender)
        {
            this._repo = repo;
            _userRepo = userRepo;
            _sender = sender;
        }
        public async Task<CommandResponse> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response = new CommandResponse();
            try
            {
                List<Guid> admins = await _repo.GetAdminsOfChat(request.ChatId);

                if (admins.Contains(request.UserId))
                {
                    await _repo.DeleteChat(request.ChatId);
                    response.Successfull = true;
                    response.ResultMessage = $"Successfully deleted chat {request.ChatId}";
                }


                response.Successfull = false;
                response.ResultMessage = ExceptionMessages.Unathorized;
            }
            catch(ExpectedException exc)
            {
                response.Successfull = false;
                response.ResultMessage = exc.Message;
            }
            catch
            {
                response.Successfull = false;
                response.ResultMessage = ExceptionMessages.ServerError;
            }

            await SendUpdateMessage(request);

            return response;
        }

        private async Task SendUpdateMessage(DeleteChatCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateChatDeletedMessage(userName);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
