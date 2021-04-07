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

namespace Infrastructure.Authentication
{
    public class JWTokenManager : IJWTokenManager
    {
        private readonly string key;
        private readonly IUserRepo _repo;
        private readonly IEncrypter _encrypter;

        public JWTokenManager(IConfiguration config, IUserRepo repo, IEncrypter encrypter)
        {
            this.key = config["EncryptionKey"];
            this._repo = repo;
            this._encrypter = encrypter;
        }
        public async Task<string> Authorize(string username, string password, CancellationToken cancellation)
        {
            User user = await _repo.FindUserByUsername(username);

            string encryptedPassword = await _encrypter.Encrypt(password);

            if(user.IsNotNull()&&user.PasswordHash.Equals(encryptedPassword))
            {
                var TokenHandler = new JwtSecurityTokenHandler();
                var TokenKey = Encoding.ASCII.GetBytes(key);
                var TokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    }),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(TokenKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = TokenHandler.CreateToken(TokenDescriptor);
                return TokenHandler.WriteToken(token);
            }
            else
            {
                return null;
            }
        }
    }
}
