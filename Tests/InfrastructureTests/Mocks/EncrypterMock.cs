using System.Threading.Tasks;
using Infrastructure.Contracts;

namespace InfrastructureTests.Mocks;

public class EncrypterMock : IEncrypter
{
    public Task<string> Encrypt(string ToBeEncrypt)
    {
        return Task.FromResult(ToBeEncrypt);
    }
}