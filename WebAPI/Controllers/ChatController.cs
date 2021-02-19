using Application.CQRS.Commands;
using Application.CQRS.Queries;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatDisplayModel>> GetChat([FromRoute] string id, CancellationToken cancellation)
        {
            GetChatQuery query = new GetChatQuery
            {
                ChatId = id
            };

            var result = await _mediator.Send(query, cancellation);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
        [HttpGet("search/{term?}/{maxCount?}")]
        public async Task<ActionResult<ChatsModel>> SearchForChat([FromRoute] string term, [FromRoute] int? maxCount, CancellationToken cancellation)
        {
            var options = new ChatSelectionOptions
            {
                MaxCount = maxCount ?? -1,
                SearchTerm = term
            };

            GetChatsQuery query = new GetChatsQuery
            {
                Options = options
            };

            var result = await _mediator.Send(query, cancellation);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteChat([FromRoute] Guid id, CancellationToken cancellation)
        {
            DeleteChatCommand command = new DeleteChatCommand
            {
                ChatId = id
            };

            bool beenDeleted = await _mediator.Send(command, cancellation);

            if (beenDeleted)
            {
                return Ok(id);
            }

            return BadRequest();
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<bool>> CreateChat([FromBody]RegisterChatCommand command, CancellationToken cancellation)
        {
            bool beenCreated = await _mediator.Send(command, cancellation);

            if (beenCreated)
            {
                return Ok();
            }

            return BadRequest();
        }

    }
}
