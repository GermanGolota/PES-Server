﻿using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Response;

namespace Application.Contracts;

public interface IJWTokenManager
{
    Task<JWTokenModel> Authorize(string username, string password, CancellationToken cancellation);
    Task<JWTokenModel> Refresh(string token, string refreshToken, CancellationToken cancellation);
}