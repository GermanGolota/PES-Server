using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.DTOs.UpdateMessages;

namespace Application.CQRS.Commands;

public class EditMessageHandler : PesCommand<EditMessageCommand>
{
    private readonly IMessageRepo _repo;
    private readonly IMessageSender _sender;
    private readonly IUserRepo _userRepo;

    public EditMessageHandler(IMessageRepo repo, IMessageSender sender, IUserRepo userRepo)
    {
        _repo = repo;
        _sender = sender;
        _userRepo = userRepo;
    }

    public override string SuccessMessage => "Successfullt edited";

    public override async Task Run(EditMessageCommand request, CancellationToken token)
    {
        await _repo.EditMessage(request.MessageId, request.UpdatedMessage);
        await SendUpdateMessage(request);
    }

    private async Task SendUpdateMessage(EditMessageCommand request)
    {
        string userName = await _userRepo.GetUsersUsername(request.UserId);
        MessageEditUpdateMessage updateMessage =
            UpdateMessageFactory.CreateMessageEditedUpdate(userName, request.UpdatedMessage);
        await _sender.SendMessageToSocket(updateMessage, request.ChatId);
    }
}