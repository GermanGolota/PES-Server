using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Queries;

public class GetMyChatsQuery : IRequest<ChatsModel>
{
    public Guid UserId { get; set; }
    public ChatSelectionOptions Options { get; set; }
}