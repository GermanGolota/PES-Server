using System;
using MediatR;
using Application.DTOs;
using System.Threading.Tasks;
using System.Threading;
using Core.Exceptions;
using Application.Contracts;
using Application.DTOs.UpdateMessages;

namespace Application.CQRS.Commands
{
    class LeaveChatHandler : IRequestHandler<LeaveChatCommand, CommandResponse>
    {
        private readonly IChatMembersService _membersService;
        private readonly IMessageSender _sender;
        private readonly IUserRepo _userRepo;

        public LeaveChatHandler(IChatMembersService membersService,IMessageSender sender, IUserRepo userRepo)
        {
            _membersService = membersService;
            _sender = sender;
            _userRepo = userRepo;
        }
        public async Task<CommandResponse> Handle(LeaveChatCommand request,
            CancellationToken cancellationToken)
        {
            CommandResponse response;
            try
            {
                await _membersService.RemoveUserFromChat(request.ChatId, request.UserId);
                response = CommandResponse.CreateSuccessfull($"Sucessfully left chat {request.ChatId}");
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

        private async Task SendUpdateMessage(LeaveChatCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateUserLeftUpdate(userName);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
