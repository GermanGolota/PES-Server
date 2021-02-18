using Application.DTOs;
using MediatR;

namespace Application.CQRS.Queries
{
    public class GetChatQuery : IRequest<ChatDisplayModel>
    {
        public string ChatId { get; set; }
    }
}
