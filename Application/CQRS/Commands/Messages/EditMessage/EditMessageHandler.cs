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
    public class EditMessageHandler : IRequestHandler<EditMessageCommand, EditMessageResponse>
    {
        private readonly IMessageRepo _repo;

        public EditMessageHandler(IMessageRepo repo)
        {
            this._repo = repo;
        }
        public async Task<EditMessageResponse> Handle(EditMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repo.EditMessage(request.UserId, request.ChatId, request.UpdatedMessage);
            }
            catch(ExpectedException exc)
            {
                return new EditMessageResponse
                {
                    SuccessfullyEdited = false,
                    Message = exc.Message
                };
            }
            catch
            {
                return new EditMessageResponse
                {
                    SuccessfullyEdited = false,
                    Message = "Something went wrong"
                };
            }

            return new EditMessageResponse
            {
                SuccessfullyEdited = true,
                Message = "Successfullt edited"
            };
        }
    }
}
