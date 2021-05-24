using System;
using System.Collections.Generic;
using MediatR;
using System.Text;
using Application.DTOs;
using System.Threading.Tasks;
using System.Threading;
using Core.Exceptions;
using Application.Contracts.Repositories;
using Application.Contracts.Service;

namespace Application.CQRS.Commands
{
    public class UnregisterUserHandler : IRequestHandler<UnregisterUserCommand, CommandResponse>
    {
        private readonly IUserRepo _repo;
        private readonly IChatMembersService _chatMembersService;

        public UnregisterUserHandler(IUserRepo repo, IChatMembersService chatMembersService)
        {
            this._repo = repo;
            _chatMembersService = chatMembersService;
        }
        public async Task<CommandResponse> Handle(UnregisterUserCommand request,
            CancellationToken cancellationToken)
        {
            return await CommandRunner.Run(request, async request =>
             {
                 var id = request.UserId;
                 await _repo.RemoveUser(id);
                 await _chatMembersService.RemoveUserFromAllChats(id);
             }, "Successfully removed user");
        }
    }
}
