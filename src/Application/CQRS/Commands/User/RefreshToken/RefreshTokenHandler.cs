using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs.Response;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IJWTokenManager _jwTokenManager;

        public RefreshTokenHandler(IJWTokenManager jwTokenManager)
        {
            _jwTokenManager = jwTokenManager;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            RefreshTokenResponse response = new RefreshTokenResponse();
            try
            {
                var token = await _jwTokenManager.Refresh(request.Token, request.RefreshToken, cancellationToken);
                response.Successfull = true;
                response.Token = token;
            }
            catch
            {
                response.Successfull = false;
            }
            return response;
        }
    }
}
