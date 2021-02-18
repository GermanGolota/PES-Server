using Application.CQRS.Commands;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

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
        [HttpPost("add")]
        public async Task<ActionResult<PostMessageResponse>> PostMessage([FromBody] PostMessageCommand command,
            CancellationToken cancellation)
        {
            var result = await _mediator.Send(command, cancellation);

            if(result.SuccessfullyPosted)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> EditMessage([FromBody] EditMessageCommand command, CancellationToken cancellation)
        {
            var response = await _mediator.Send(command, cancellation);

            if(response.SuccessfullyEdited)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
