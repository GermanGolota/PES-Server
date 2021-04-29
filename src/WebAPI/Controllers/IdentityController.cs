using Application.CQRS.Commands;
using Application.CQRS.Queries;
using Application.DTOs;
using Application.DTOs.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IdentityController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<JWTokenModel>> RegisterUser([FromBody] RegisterUserCommand command, CancellationToken cancellation)
        {
            JWTokenModel token = await _mediator.Send(command, cancellation);

            if (token is null)
            {
                return BadRequest();
            }

            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<JWTokenModel>> LoginUser([FromBody] LoginUserQuery query, CancellationToken cancellation)
        {
            JWTokenModel token = await _mediator.Send(query, cancellation);

            if (token is null)
            {
                return BadRequest();
            }

            return Ok(token);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellation)
        {
            RefreshTokenResponse token = await _mediator.Send(command, cancellation);
            ActionResult<RefreshTokenResponse> result;
            if (token.Successfull)
            {
                result = Ok(token);
            }
            else
            {
                result = BadRequest(token);
            }
            return result;
        }

        [Authorize]
        [HttpDelete("unregister")]
        public async Task<ActionResult<CommandResponse>> UnregisterUser(CancellationToken cancellation)
        {
            var command = new UnregisterUserCommand
            {
                UserId = this.GetUserId()
            };

            CommandResponse response = await _mediator.Send(command, cancellation);

            return this.CommandResponse(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<CommandResponse>> LogoutUser(CancellationToken cancellation)
        {
            var command = new LogoutCommand
            {
                UserId = this.GetUserId()
            };

            CommandResponse response = await _mediator.Send(command, cancellation);

            return this.CommandResponse(response);
        }
    }
}
