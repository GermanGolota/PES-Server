using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Repositories;
using Application.DTOs.Response;
using Core.Entities;
using Core.Extensions;
using Infrastructure.Config;
using Infrastructure.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class JWTokenManager : IJWTokenManager
{
    private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature;
    private readonly IEncrypter _encrypter;
    private readonly string _key;
    private readonly IUserRepo _repo;

    public JWTokenManager(TokenConfig config, IUserRepo repo, IEncrypter encrypter)
    {
        _key = config.EncryptionKey;
        _repo = repo;
        _encrypter = encrypter;
    }

    public async Task<JWTokenModel> Authorize(string username, string password, CancellationToken cancellation)
    {
        User user = await _repo.FindUserByUsername(username);
        string encryptedPassword = await _encrypter.Encrypt(password);

        JWTokenModel result = null;
        if (user.IsNotNull() && user.PasswordHash.Equals(encryptedPassword))
        {
            List<Claim> claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.UserId.ToString()) };

            result = CreateTokenForClaims(claims);
            string refreshToken = GenerateRefreshToken();
            await _repo.SetRefreshTokenFor(user.UserId, refreshToken);
            result.RefreshToken = refreshToken;
        }

        return result;
    }

    public async Task<JWTokenModel> Refresh(string token, string refreshToken, CancellationToken cancellation)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token);
        JWTokenModel result = null;
        if (principal.HasClaim(x => x.Type.Equals(ClaimTypes.NameIdentifier)))
        {
            Guid userId = GetUserId(principal);
            string userRefreshToken = await _repo.GetRefreshTokenFor(userId);
            if (userRefreshToken != token) throw new SecurityTokenException("Invalid refresh token");

            string newRefreshToken = GenerateRefreshToken();
            await _repo.SetRefreshTokenFor(userId, newRefreshToken);
            result = CreateTokenForClaims(principal.Claims);
            result.RefreshToken = newRefreshToken;
        }

        return result;
    }

    private JWTokenModel CreateTokenForClaims(IEnumerable<Claim> claims)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] tokenKey = Encoding.ASCII.GetBytes(_key);
        DateTime expires = DateTime.UtcNow.AddDays(1);
        SigningCredentials credentials = new(new SymmetricSecurityKey(tokenKey), SecurityAlgorithm);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = credentials
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        DateTimeOffset offset = new(expires);
        long expiresIn = offset.ToUnixTimeSeconds();

        return new JWTokenModel
        {
            AccessToken = tokenHandler.WriteToken(token),
            ExpirationStamp = expiresIn
        };
    }

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private Guid GetUserId(ClaimsPrincipal principal)
    {
        string userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid result = Guid.Empty;
        if (!string.IsNullOrEmpty(userId)) result = new Guid(userId);
        return result;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            ValidateLifetime = false //important
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal =
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (BadToken(securityToken)) throw new SecurityTokenException("Invalid token");

        return principal;
    }

    private bool BadToken(SecurityToken securityToken)
    {
        return !(securityToken is JwtSecurityToken jwtSecurityToken) || WrongEncryptionAlgorithm(jwtSecurityToken);
    }

    private bool WrongEncryptionAlgorithm(JwtSecurityToken jwtSecurityToken)
    {
        return !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithm, StringComparison.InvariantCultureIgnoreCase);
    }
}