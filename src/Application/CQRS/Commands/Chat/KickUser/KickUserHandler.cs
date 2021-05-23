using System;
using System.Collections.Generic;
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
    public class KickUserHandler : IRequestHandler<KickUserCommand, CommandResponse>
    {
        private readonly IUserRepo _userRepo;
        private readonly IMessageSender _sender;
        private readonly IChatMembersService _membersService;

        public KickUserHandler(IUserRepo userRepo, IMessageSender sender, IChatMembersService membersService)
        {
            _userRepo = userRepo;
            _sender = sender;
            _membersService = membersService;
        }

        public async Task<CommandResponse> Handle(KickUserCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response = new CommandResponse();
            try
            {
                List<Guid> admins = await _membersService.GetAdminsOfChat(request.ChatId);

                if (admins.Contains(request.UserId))
                {
                    await _membersService.Kick(request.ChatId, request.UserId);
                    response.Successfull = true;
                    response.ResultMessage = $"Successfully kicked user {request.UserId} in chat {request.ChatId}";
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

        private async Task SendUpdateMessage(KickUserCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateUserKickedUpdate(userName);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
