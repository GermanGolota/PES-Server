using System;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Queries;

public class GetChatsQuery : IRequest<ChatsModel>
{
    public ChatSelectionOptions Options { get; set; }
    public Guid UserId { get; set; }
}