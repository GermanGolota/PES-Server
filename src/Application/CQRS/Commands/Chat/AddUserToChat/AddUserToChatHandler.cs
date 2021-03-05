using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Application.DTOs.UpdateMessages;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class AddUserToChatHandler : IRequestHandler<AddUserToChatCommand, CommandResponse>
    {
        private readonly IChatRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly IMessageSender _sender;

        public AddUserToChatHandler(IChatRepo repo, IUserRepo userRepo, IMessageSender sender)
        {
            this._repo = repo;
            _userRepo = userRepo;
            _sender = sender;
        }
        public async Task<CommandResponse> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response;
            try
            {
                await _repo.AddUser(request.ChatId, request.UserId, request.Password);
                response = CommandResponse.CreateSuccessfull("Succesfully added user to chat");
            }
            catch(ExpectedException exc)
            {
                response = CommandResponse.CreateUnsuccessfull(exc.Message);
            }
            catch
            {
                response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.ServerError);
            }

            await SendUpdateMessage(request);

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
