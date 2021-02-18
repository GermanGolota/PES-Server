using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using MediatR;

namespace Application.CQRS.Commands
{
    public class EditMessageHandler : IRequestHandler<EditMessageCommand, EditMessageResponse>
    {
        private readonly IMessageRepo _repo;

        public EditMessageHandler(IMessageRepo repo)
        {
            this._repo = repo;
        }
        public async Task<EditMessageResponse> Handle(EditMessageCommand request, CancellationToken cancellationToken)
        {
            
        }
    }
}
