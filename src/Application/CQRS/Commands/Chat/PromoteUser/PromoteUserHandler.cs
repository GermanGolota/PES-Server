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
    public class PromoteUserHandler : PesCommand<PromoteUserCommand>
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

        private string _successMessage;
        public override string SuccessMessage => _successMessage;

        public override async Task<bool> Authorize(PromoteUserCommand request, CancellationToken token)
        {
            List<Guid> admins = await _membersService.GetAdminsOfChat(request.ChatId);

            return admins.Contains(request.RequesterId);
        }

        public override async Task Run(PromoteUserCommand request, CancellationToken token)
        {
            await _membersService.PromoteToAdmin(request.ChatId, request.UserId);
            await SendUpdateMessage(request);
            _successMessage = $"Successfully promoted user {request.UserId} in chat {request.ChatId}";
        }

        private async Task SendUpdateMessage(PromoteUserCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateUserPromotedUpdate(userName);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
