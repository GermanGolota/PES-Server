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
    public class PromoteUserHandler : IRequestHandler<PromoteUserCommand, CommandResponse>
    {
        private readonly IChatRepo _repo;

        public PromoteUserHandler(IChatRepo repo)
        {
            this._repo = repo;
        }
        public async Task<CommandResponse> Handle(PromoteUserCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response = new CommandResponse();
            try
            {
                List<Guid> admins = await _repo.GetAdminsOfChat(request.ChatId);

                if (admins.Contains(request.UserId))
                {
                    await _repo.PromoteToAdmin(request.ChatId, request.UserId);
                    response.Successfull = true;
                    response.ResultMessage = $"Successfully promoted user {request.UserId} in chat {request.ChatId}";
                }

                response.Successfull = false;
                response.ResultMessage = ExceptionMessages.Unathorized;
            }
            catch (ExpectedException exc)
            {
                response.Successfull = false;
                response.ResultMessage = exc.Message;
            }
            catch
            {
                response.Successfull = false;
                response.ResultMessage = ExceptionMessages.ServerError;
            }

            return response;
        }
    }
}
