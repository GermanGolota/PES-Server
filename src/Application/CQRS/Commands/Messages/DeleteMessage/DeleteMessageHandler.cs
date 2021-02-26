using Application.Contracts;
using Application.DTOs;
using Core.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Commands
{
    public class DeleteMessageHandler : IRequestHandler<DeleteMessageCommand, CommandResponse>
    {
        private readonly IMessageRepo _repo;

        public DeleteMessageHandler(IMessageRepo repo)
        {
            this._repo = repo;
        }
        public async Task<CommandResponse> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response;
            try
            {
                await _repo.DeleteMessage(request.UserId, request.ChatId);
                response = CommandResponse.CreateSuccessfull("Successfully deleted message");
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
