using Application.CQRS.Commands;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.RequestModels;
using WebAPI.Extensions;

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
        public async Task<ActionResult<PostMessageResponse>> PostMessage([FromBody] PostMessageRequest request,
            CancellationToken cancellation)
        {
            PostMessageCommand command = new PostMessageCommand
            {
                ChatId = request.ChatId,
                Message = request.Message,
                UserId = this.GetUserId()
            };

            var result = await _mediator.Send(command, cancellation);

            if(result.SuccessfullyPosted)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [Authorize]
        [HttpPut("edit")]
        public async Task<IActionResult> EditMessage([FromBody] EditMessageRequest request, CancellationToken cancellation)
        {
            EditMessageCommand command = new()
            {
                ChatId = request.ChatId,
                UpdatedMessage = request.UpdatedMessage,
                UserId = this.GetUserId()
            };

            var response = await _mediator.Send(command, cancellation);

            if(response.SuccessfullyEdited)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

    }
}
