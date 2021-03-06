﻿using System;
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
    public class DeleteChatHandler : PesCommand<DeleteChatCommand>
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

        public override string SuccessMessage => $"Successfully deleted chat";

        public override async Task<bool> Authorize(DeleteChatCommand request, CancellationToken token)
        {
            Guid creatorId = await _membersService.GetChatCreator(request.ChatId);
            return creatorId.Equals(request.UserId);
        }

        public override async Task Run(DeleteChatCommand request, CancellationToken token)
        {

            await _repo.DeleteChat(request.ChatId);
            await SendUpdateMessage(request);
        }

        private async Task SendUpdateMessage(DeleteChatCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateChatDeletedMessage(userName);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
