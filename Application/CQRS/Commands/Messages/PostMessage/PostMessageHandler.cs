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
    public class PostMessageHandler : IRequestHandler<PostMessageCommand, PostMessageResponse>
    {
        private readonly IMessageRepo _repo;

        public PostMessageHandler(IMessageRepo repo)
        {
            this._repo = repo;
        }
        public async Task<PostMessageResponse> Handle(PostMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repo.AddMessageToChat(request.Message, request.ChatId, request.UserId);
            }
            catch(ExpectedException exc)
            {
                return new PostMessageResponse
                {
                    ResultMessage = exc.Message,
                    SuccessfullyPosted = false
                };
            }
            catch
            {
                return new PostMessageResponse
                {
                    ResultMessage = "Something went wrong",
                    SuccessfullyPosted = false
                };
            }

            return new PostMessageResponse
            {
                ResultMessage = "Success",
                SuccessfullyPosted = true
            };
        }
    }
}
