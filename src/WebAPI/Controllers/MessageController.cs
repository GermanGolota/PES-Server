using Application.CQRS.Commands;
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
    [ApiController]
    [Route("api/v1/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [Authorize]
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

            CommandResponse result = await _mediator.Send(command, cancellation);

            if(result.Successfull)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [Authorize]
        [HttpPut("edit")]
        public async Task<ActionResult<CommandResponse>> EditMessage([FromBody] EditMessageRequest request, 
            CancellationToken cancellation)
        {
            EditMessageCommand command = new()
            {
                ChatId = request.ChatId,
                UpdatedMessage = request.UpdatedMessage,
                UserId = this.GetUserId()
            };

            CommandResponse response = await _mediator.Send(command, cancellation);

            if(response.Successfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [Authorize]
        [HttpDelete("delete/{chatId}")]
        public async Task<ActionResult<CommandResponse>> DeleteMessage([FromRoute] string chatId,
            CancellationToken cancellation)
        {
            var command = new DeleteMessageCommand
            {
                ChatId = new Guid(chatId),
                UserId = this.GetUserId()
            };

            CommandResponse response = await _mediator.Send(command, cancellation);

            if(response.Successfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

    }
}
