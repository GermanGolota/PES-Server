﻿using Application.CQRS.Commands;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.RequestModels;
using WebAPI.Extensions;
using System;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost("add")]
        public async Task<ActionResult<CommandResponse>> PostMessage([FromBody] PostMessageRequest request,
            CancellationToken cancellation)
        {
            PostMessageCommand command = new PostMessageCommand
            {
                ChatId = request.ChatId,
                Message = request.Message,
                UserId = this.GetUserId()
            };

            CommandResponse response = await _mediator.Send(command, cancellation);

            return this.CommandResponse(response);
        }

        [HttpPut("edit")]
        public async Task<ActionResult<CommandResponse>> EditMessage([FromBody] EditMessageRequest request, 
            CancellationToken cancellation)
        {
            EditMessageCommand command = new()
            {
                ChatId = request.ChatId,
                UpdatedMessage = request.UpdatedMessage,
                UserId = this.GetUserId(),
                MessageId = request.MessageId
            };

            CommandResponse response = await _mediator.Send(command, cancellation);

            return this.CommandResponse(response);
        }

        [HttpDelete("delete/{chatId}/{messageId}")]
        public async Task<ActionResult<CommandResponse>> DeleteMessage([FromRoute] string chatId,
            [FromRoute] string messageId, CancellationToken cancellation)
        {
            var command = new DeleteMessageCommand
            {
                ChatId = new Guid(chatId),
                UserId = this.GetUserId(),
                MessageId = new Guid(messageId)
            };

            CommandResponse response = await _mediator.Send(command, cancellation);

            return this.CommandResponse(response);
        }

    }
}
