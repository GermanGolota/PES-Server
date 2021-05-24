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
    public class PostMessageHandler : IRequestHandler<PostMessageCommand, CommandResponse>
    {
        private readonly IMessageRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly IMessageSender _sender;

        public PostMessageHandler(IMessageRepo repo, IUserRepo userRepo, IMessageSender sender)
        {
            this._repo = repo;
            _userRepo = userRepo;
            _sender = sender;
        }
        public async Task<CommandResponse> Handle(PostMessageCommand request, CancellationToken cancellationToken)
        {
            return await CommandRunner.Run(request, async request =>
            {
                await _repo.AddMessageToChat(request.Message, request.ChatId, request.UserId);
                await SendUpdateMessage(request);
            }, "Success");
        }

        private async Task SendUpdateMessage(PostMessageCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateMessageCreatedUpdate(userName, request.Message);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
