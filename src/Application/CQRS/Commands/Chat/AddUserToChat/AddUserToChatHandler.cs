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
    public class AddUserToChatHandler : IRequestHandler<AddUserToChatCommand, CommandResponse>
    {
        private readonly IUserRepo _userRepo;
        private readonly IMessageSender _sender;
        private readonly IChatMembersService _membersService;

        public AddUserToChatHandler(IUserRepo userRepo, IMessageSender sender,
            IChatMembersService membersService)
        {
            _userRepo = userRepo;
            _sender = sender;
            _membersService = membersService;
        }
        public async Task<CommandResponse> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
        {
            var response = await CommandRunner.Run(async () =>
            {
                await _membersService.AddUser(request.ChatId, request.UserId, request.Password);
                await SendUpdateMessage(request);
            }, "Succesfully added user to chat");
            return response;
        }

        private async Task SendUpdateMessage(AddUserToChatCommand request)
        {
            string userName = await _userRepo.GetUsersUsername(request.UserId);
            var updateMessage = UpdateMessageFactory.CreateUserJoinedUpdate(userName);
            await _sender.SendMessageToSocket(updateMessage, request.ChatId);
        }
    }
}
