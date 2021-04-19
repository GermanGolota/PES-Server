using Infrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureTests.Mocks
{
    public class EncrypterMock : IEncrypter
    {
        public Task<string> Encrypt(string ToBeEncrypt)
        {
            return Task.FromResult(ToBeEncrypt);
        }
    }
}
