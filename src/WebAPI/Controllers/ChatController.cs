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
using Core.Extensions;
using System.IO;

namespace WebAPI.Controllers
{
    [Authorize]
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
                ChatId = id,
                RequesterId = this.GetUserId()
            };

            var result = await _mediator.Send(query, cancellation);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("search/{page?}/{maxCount?}/{term?}/{isMultiMessage?}")]
        public async Task<ActionResult<ChatsModel>> SearchForChat([FromRoute] int? page, [FromRoute] int? maxCount,
            [FromRoute] string term, [FromRoute] bool? isMultiMessage, CancellationToken cancellation)
        {
            var options = new ChatSelectionOptions
            {
                ChatsPerPage = maxCount ?? -1,
                SearchTerm = term,
                PageNumber = page ?? 1,
                MultiMessage = ConvertToMode(isMultiMessage)
            };

            GetChatsQuery query = new GetChatsQuery
            {
                Options = options,
                UserId = this.GetUserId()
            };

            var result = await _mediator.Send(query, cancellation);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        private ChatMultiMessageMode ConvertToMode(bool? isMultiMessage)
        {
            ChatMultiMessageMode multiMessage;

            if (isMultiMessage.HasValue)
            {
                if (isMultiMessage.Value)
                {
                    multiMessage = ChatMultiMessageMode.MultiMessage;
                }
                else
                {
                    multiMessage = ChatMultiMessageMode.SingleMessage;
                }
            }
            else
            {
                multiMessage = ChatMultiMessageMode.Any;
            }

            return multiMessage;
        }

        [HttpGet("search/my/{page?}/{maxCount?}/{term?}/{isMultiMessage?}")]
        public async Task<ActionResult<ChatsModel>> GetMyChats([FromRoute] int? page, [FromRoute] int? maxCount,
            [FromRoute] string term, [FromRoute] bool? isMultiMessage, CancellationToken cancellation)
        {
            var options = new ChatSelectionOptions
            {
                ChatsPerPage = maxCount ?? -1,
                SearchTerm = term,
                PageNumber = page ?? 1,
                MultiMessage = ConvertToMode(isMultiMessage)
            };

            GetMyChatsQuery query = new GetMyChatsQuery
            {
                UserId = this.GetUserId(),
                Options = options
            };

            var result = await _mediator.Send(query, cancellation);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CommandResponse>> DeleteChat([FromRoute] Guid id, CancellationToken cancellation)
        {
            DeleteChatCommand command = new DeleteChatCommand
            {
                ChatId = id,
                UserId = this.GetUserId()
            };

            var response = await _mediator.Send(command, cancellation);

            return this.CommandResponse(response);
        }

        [HttpPost("create")]
        public async Task<ActionResult<CommandResponse>> CreateChat([FromBody] CreateChatRequest request, CancellationToken cancellation)
        {
            RegisterChatCommand command = new()
            {
                AdminId = this.GetUserId(),
                ChatName = request.ChatName,
                ChatPassword = request.ChatPassword,
                IsMultiMessage = request.IsMultiMessage
            };

            var response = await _mediator.Send(command, cancellation);

            return this.CommandResponse(response);
        }

        [HttpPost("{chatId}/join")]
        public async Task<ActionResult<CommandResponse>> JoinChat([FromRoute] string chatId,
            [FromBody] JoinChatRequest request, CancellationToken cancellation)
        {
            AddUserToChatCommand command = new()
            {
                ChatId = new Guid(chatId),
                UserId = this.GetUserId(),
                Password = request.Password
            };

            var response = await _mediator.Send(command, cancellation);

            return this.CommandResponse(response);
        }

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

        [HttpPost("{chatId}/admin/promote/{userId}")]
        public async Task<ActionResult<CommandResponse>> PromoteUser([FromRoute] string chatId,
            [FromRoute] string userId, CancellationToken cancellation)
        {
            PromoteUserCommand command = new PromoteUserCommand
            {
                ChatId = new Guid(chatId),
                UserId = new Guid(userId),
                RequesterId = this.GetUserId()
            };

            var response = await _mediator.Send(command, cancellation);
            return this.CommandResponse(response);
        }

        [HttpPost("{chatId}/admin/kick/{userId}")]
        public async Task<ActionResult<CommandResponse>> KickUser([FromRoute] string chatId,
           [FromRoute] string userId, CancellationToken cancellation)
        {
            KickUserCommand command = new KickUserCommand
            {
                ChatId = new Guid(chatId),
                UserId = new Guid(userId),
                RequesterId = this.GetUserId()
            };

            var response = await _mediator.Send(command, cancellation);
            return this.CommandResponse(response);
        }

        [HttpPost("{chatId}/leave")]
        public async Task<ActionResult<CommandResponse>> LeaveChat([FromRoute] string chatId,
            CancellationToken cancellation)
        {
            LeaveChatCommand command = new LeaveChatCommand
            {
                ChatId = new Guid(chatId),
                UserId = this.GetUserId()
            };

            var response = await _mediator.Send(command, cancellation);
            return this.CommandResponse(response);
        }

        /// <summary>
        /// Updates chats image
        /// Requires file being passed as form data
        /// </summary>
        [HttpPost("{chatId}/admin/uploadImage")]
        public async Task<ActionResult<CommandResponse>> UpdateChatImage([FromRoute] string chatId,
            CancellationToken cancellation)
        {
            CommandResponse response;

            var files = Request.Form.Files;
            if (files.IsNotNull() && files.Count > 0)
            {
                var file = files[0];
                string extension = Path.GetExtension(file.FileName);
                if (extension.StartsWith("."))
                {
                    extension = extension.Substring(1);
                }
                UploadImageCommand command = new UploadImageCommand
                {
                    ChatId = new Guid(chatId),
                    FileExtension = extension,
                    ImageStream = file.OpenReadStream(),
                    RequesterId = this.GetUserId()
                };

                response = await _mediator.Send(command, cancellation);
            }
            else
            {
                response = CommandResponse.CreateUnsuccessfull("No file is attached for upload");
            }
            return this.CommandResponse(response);
        }
    }
}
