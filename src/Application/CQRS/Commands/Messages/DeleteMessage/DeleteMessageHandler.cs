using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.DTOs.UpdateMessages;

namespace Application.CQRS.Commands;

public class DeleteMessageHandler : PesCommand<DeleteMessageCommand>
{
    private readonly IMessageRepo _repo;
    private readonly IMessageSender _sender;
    private readonly IUserRepo _userRepo;

    public DeleteMessageHandler(IMessageRepo repo, IUserRepo userRepo, IMessageSender sender)
    {
        _repo = repo;
        _userRepo = userRepo;
        _sender = sender;
    }

    public override string SuccessMessage => "Successfully deleted message";

    public override async Task Run(DeleteMessageCommand request, CancellationToken token)
    {
        await _repo.DeleteMessage(request.MessageId);
        await SendUpdateMessage(request);
    }

    private async Task SendUpdateMessage(DeleteMessageCommand request)
    {
        string username = await _userRepo.GetUsersUsername(request.UserId);
        MessageDeletionUpdateMessage updateMessage = UpdateMessageFactory.CreateMessageDeletedUpdate(username);
        await _sender.SendMessageToSocket(updateMessage, request.ChatId);
    }
}