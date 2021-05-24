using System;
using System.Collections.Generic;
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
    public class PromoteUserHandler : IRequestHandler<PromoteUserCommand, CommandResponse>
    {
        private readonly IUserRepo _userRepo;
        private readonly IMessageSender _sender;
        private readonly IChatMembersService _membersService;

        public PromoteUserHandler(IUserRepo userRepo, IMessageSender sender, IChatMembersService membersService)
        {
            _userRepo = userRepo;
            _sender = sender;
            _membersService = membersService;
        }
        public async Task<CommandResponse> Handle(PromoteUserCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response = await CommandRunner.Run(async () =>
            {
                await _membersService.PromoteToAdmin(request.ChatId, request.UserId);
                await SendUpdateMessage(request);
            }, $"Successfully promoted user {request.UserId} in chat {request.ChatId}",
            async () =>
            {
                List<Guid> admins = await _membersService.GetAdminsOfChat(request.ChatId);

                return admins.Contains(request.RequesterId);
            });
            return response;
        }

        private async Task SendUpdateMessage(PromoteUserCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateUserPromotedUpdate(userName);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
