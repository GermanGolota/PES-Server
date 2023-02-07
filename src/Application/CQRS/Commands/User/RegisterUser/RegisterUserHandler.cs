using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.DTOs;
using Application.DTOs.Response;
using MediatR;

namespace Application.CQRS.Commands;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, JWTokenModel>
{
    private readonly IUserRepo _repo;
    private readonly IJWTokenManager _tokenManager;

    public RegisterUserHandler(IUserRepo identity, IJWTokenManager jWTokenManager)
    {
        _repo = identity;
        _tokenManager = jWTokenManager;
    }


    public async Task<JWTokenModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        //TODO:Automapper
        UserRegistrationModel user = new()
        {
            Username = request.Username,
            Password = request.Password,
            PesKey = request.PesKey
        };
        await _repo.AddUser(user);

        JWTokenModel response = await _tokenManager.Authorize(user.Username, user.Password, cancellationToken);

        return response;
    }
}