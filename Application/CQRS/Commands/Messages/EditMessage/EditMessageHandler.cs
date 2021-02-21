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
    public class EditMessageHandler : IRequestHandler<EditMessageCommand, CommandResponse>
    {
        private readonly IMessageRepo _repo;

        public EditMessageHandler(IMessageRepo repo)
        {
            this._repo = repo;
        }
        public async Task<CommandResponse> Handle(EditMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repo.EditMessage(request.UserId, request.ChatId, request.UpdatedMessage);
            }
            catch(ExpectedException exc)
            {
                return new CommandResponse
                {
                    Successfull = false,
                    ResultMessage = exc.Message
                };
            }
            catch
            {
                return new CommandResponse
                {
                    Successfull = false,
                    ResultMessage = "Something went wrong"
                };
            }

            return new CommandResponse
            {
                Successfull = true,
                ResultMessage = "Successfullt edited"
            };
        }
    }
}
