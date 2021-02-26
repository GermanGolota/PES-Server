using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class AddUserToChatHandler : IRequestHandler<AddUserToChatCommand, CommandResponse>
    {
        private readonly IChatRepo _repo;

        public AddUserToChatHandler(IChatRepo repo)
        {
            this._repo = repo;
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
            return response;
        }
    }
}
