using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.Contracts.Service;
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
        private readonly IChatMembersService _membersService;

        public DeleteChatHandler(IChatRepo repo, IUserRepo userRepo, IMessageSender sender, IChatMembersService membersService)
        {
            this._repo = repo;
            _userRepo = userRepo;
            _sender = sender;
            _membersService = membersService;
        }
        public async Task<CommandResponse> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response = new CommandResponse();
            try
            {
                Guid creatorId = await _membersService.GetChatCreator(request.ChatId);

                if (creatorId.Equals(request.UserId))
                {
                    await _repo.DeleteChat(request.ChatId);
                    response.Successfull = true;
                    response.ResultMessage = $"Successfully deleted chat {request.ChatId}";
                    await SendUpdateMessage(request);
                }
                else
                {
                    response.Successfull = false;
                    response.ResultMessage = ExceptionMessages.Unathorized;
                }
            }
            catch (ExpectedException exc)
            {
                response.Successfull = false;
                response.ResultMessage = exc.Message;
            }
            catch
            {
                response.Successfull = false;
                response.ResultMessage = ExceptionMessages.ServerError;
            }

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
