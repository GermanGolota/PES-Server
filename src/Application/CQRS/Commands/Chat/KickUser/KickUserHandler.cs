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
using Core.Extensions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class KickUserHandler : PesCommand<KickUserCommand>
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

        private string _successMessage;
        public override string SuccessMessage => _successMessage;

        public override async Task<bool> Authorize(KickUserCommand request, CancellationToken token)
        {
            List<Guid> admins = await _membersService.GetAdminsOfChat(request.ChatId);
            return admins.Contains(request.RequesterId) && admins.NotContains(request.UserId);
        }

        public override async Task Run(KickUserCommand request, CancellationToken token)
        {
            await _membersService.Kick(request.ChatId, request.UserId);
            await SendUpdateMessage(request);
            _successMessage = $"Successfully kicked user {request.UserId} in chat {request.ChatId}";
        }

        private async Task SendUpdateMessage(KickUserCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateUserKickedUpdate(userName);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
