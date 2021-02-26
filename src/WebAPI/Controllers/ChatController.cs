﻿using Application.CQRS.Commands;
using Application.CQRS.Queries;
using Application.DTOs;
using Application.DTOs.Chat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.Extensions;
using WebAPI.RequestModels;

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
        [HttpGet("search/{maxCount?}/{term?}")]
        public async Task<ActionResult<ChatsModel>> SearchForChat([FromRoute] int? maxCount, [FromRoute] string term,
            CancellationToken cancellation)
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
        public async Task<ActionResult<CommandResponse>> DeleteChat([FromRoute] Guid id, CancellationToken cancellation)
        {
            DeleteChatCommand command = new DeleteChatCommand
            {
                ChatId = id,
                UserId = this.GetUserId()
            };

            var response = await _mediator.Send(command, cancellation);

            if (response.Successfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<CommandResponse>> CreateChat([FromBody] CreateChatRequest request, CancellationToken cancellation)
        {
            RegisterChatCommand command = new()
            {
                AdminId = this.GetUserId(),
                ChatName = request.ChatName,
                ChatPassword = request.ChatPassword
            };

            var response = await _mediator.Send(command, cancellation);

            if (response.Successfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [Authorize]
        [HttpPost("{chatId}/join")]
        public async Task<ActionResult<CommandResponse>> JoinChat([FromRoute] string chatId, 
            [FromBody]JoinChatRequest request, CancellationToken cancellation)
        {
            AddUserToChatCommand command = new()
            {
                ChatId = new Guid(chatId),
                UserId = this.GetUserId(),
                Password = request.Password
            };

            var response = await _mediator.Send(command, cancellation);

            if (response.Successfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [Authorize]
        [HttpGet("{chatId}/admin/getMembers")]
        public async Task<ActionResult<List<ChatMemberModelAdmin>>> GetChatMembersAdmin([FromRoute] string chatId,
            CancellationToken cancellation)
        {
            GetChatMembersAdminQuery query = new()
            {
                ChatId = new Guid(chatId),
                UserId = this.GetUserId()
            };

            return await _mediator.Send(query, cancellation);
        }
        [Authorize]
        [HttpGet("{chatId}/getMembers")]
        public async Task<ActionResult<List<ChatMemberModel>>> GetChatMembers([FromRoute] string chatId,
            CancellationToken cancellation)
        {
            GetChatMembersQuery query = new()
            {
                ChatId = new Guid(chatId),
                UserId = this.GetUserId()
            };

            return await _mediator.Send(query, cancellation);
        }

        [Authorize]
        [HttpPost("{chatId}/admin/promote/{userId}")]
        public async Task<ActionResult<CommandResponse>> PromoteUser([FromRoute] string chatId,
            string userId, CancellationToken cancellation)
        {
            PromoteUserCommand command = new PromoteUserCommand
            {
                ChatId = new Guid(chatId),
                UserId = new Guid(userId),
                RequesterId = this.GetUserId()
            };

            var response = await _mediator.Send(command, cancellation);

            if (response.Successfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}