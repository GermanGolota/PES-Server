using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;

namespace Application.CQRS.Commands;

public class RegisterChatHandler : PesCommand<RegisterChatCommand>
{
    private readonly IChatRepo _repo;

    private string _successMessage;

    public RegisterChatHandler(IChatRepo repo)
    {
        _repo = repo;
    }

    public override string SuccessMessage => _successMessage;

    public override async Task Run(RegisterChatCommand request, CancellationToken token)
    {
        string password = GetChatPassword(request);
        Guid chatId = await _repo.CreateChat(request.AdminId, request.ChatName, password, request.IsMultiMessage);
        _successMessage = $"Successfully registered chat {chatId}";
    }

    private string GetChatPassword(RegisterChatCommand request)
    {
        string output = "1";
        if (!string.IsNullOrEmpty(request.ChatPassword)) output = request.ChatPassword;
        return output;
    }
}