using Application.Contracts;
using Core.Entities;
using Infrastructure.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Application.Contracts.Repositories;
using Application.DTOs.Response;
using System.Security.Cryptography;
using System.Collections.Generic;
using Infrastructure.Config;

namespace Infrastructure.Authentication
{
    public class JWTokenManager : IJWTokenManager
    {
        private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature;
        private readonly string _key;
        private readonly IUserRepo _repo;
        private readonly IEncrypter _encrypter;

        public JWTokenManager(TokenConfig config, IUserRepo repo, IEncrypter encrypter)
        {
            this._key = config.EncryptionKey;
            this._repo = repo;
            this._encrypter = encrypter;
        }

        public async Task<JWTokenModel> Authorize(string username, string password, CancellationToken cancellation)
        {
            User user = await _repo.FindUserByUsername(username);
            string encryptedPassword = await _encrypter.Encrypt(password);

            JWTokenModel result = null;
            if (user.IsNotNull() && user.PasswordHash.Equals(encryptedPassword))
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()) };

                result = CreateTokenForClaims(claims);
                string refreshToken = GenerateRefreshToken();
                await _repo.SetRefreshTokenFor(user.UserId, refreshToken);
                result.RefreshToken = refreshToken;
            }
            return result;
        }

        private JWTokenModel CreateTokenForClaims(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key);
            var expires = DateTime.UtcNow.AddDays(1);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithm);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = credentials
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            var offset = new DateTimeOffset(expires);
            long expiresIn = offset.ToUnixTimeSeconds();

            return new JWTokenModel
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpirationStamp = expiresIn
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<JWTokenModel> Refresh(string token, string refreshToken, CancellationToken cancellation)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            JWTokenModel result = null;
            if (principal.HasClaim(x => x.Type.Equals(ClaimTypes.NameIdentifier)))
            {
                Guid userId = GetUserId(principal);
                string userRefreshToken = await _repo.GetRefreshTokenFor(userId);
                if (userRefreshToken != token)
                {
                    throw new SecurityTokenException("Invalid refresh token");
                }

                var newRefreshToken = GenerateRefreshToken();
                await _repo.SetRefreshTokenFor(userId, newRefreshToken);
                result = CreateTokenForClaims(principal.Claims);
                result.RefreshToken = newRefreshToken;
            }
            return result;
        }

        private Guid GetUserId(ClaimsPrincipal principal)
        {
            string userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid result = Guid.Empty;
            if (!String.IsNullOrEmpty(userId))
            {
                result = new Guid(userId);
            }
            return result;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateLifetime = false //important
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (BadToken(securityToken))
            {
                throw new SecurityTokenException("Invalid token");
            }

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
}
