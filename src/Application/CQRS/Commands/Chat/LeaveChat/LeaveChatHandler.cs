using System;
using MediatR;
using Application.DTOs;
using System.Threading.Tasks;
using System.Threading;
using Core.Exceptions;
using Application.Contracts;
using Application.DTOs.UpdateMessages;
using Application.Contracts.Repositories;
using Application.Contracts.Service;

namespace Application.CQRS.Commands
{
    class LeaveChatHandler : IRequestHandler<LeaveChatCommand, CommandResponse>
    {
        private readonly IChatMembersService _membersService;
        private readonly IMessageSender _sender;
        private readonly IUserRepo _userRepo;

        public LeaveChatHandler(IChatMembersService membersService, IMessageSender sender, IUserRepo userRepo)
        {
            _membersService = membersService;
            _sender = sender;
            _userRepo = userRepo;
        }

        public async Task<CommandResponse> Handle(LeaveChatCommand request,
            CancellationToken cancellationToken)
        {
            CommandResponse response = await CommandRunner.Run(async () =>
            {
                await _membersService.RemoveUserFromChat(request.ChatId, request.UserId);
                await SendUpdateMessage(request);
            }, $"Sucessfully left chat {request.ChatId}");
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
