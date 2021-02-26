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
    public class PostMessageHandler : IRequestHandler<PostMessageCommand, CommandResponse>
    {
        private readonly IMessageRepo _repo;

        public PostMessageHandler(IMessageRepo repo)
        {
            this._repo = repo;
        }
        public async Task<CommandResponse> Handle(PostMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repo.AddMessageToChat(request.Message, request.ChatId, request.UserId);
            }
            catch(ExpectedException exc)
            {
                return new CommandResponse
                {
                    ResultMessage = exc.Message,
                    Successfull = false
                };
            }
            catch
            {
                return new CommandResponse
                {
                    ResultMessage = "Something went wrong",
                    Successfull = false
                };
            }

            return new CommandResponse
            {
                ResultMessage = "Success",
                Successfull = true
            };
        }
    }
}
