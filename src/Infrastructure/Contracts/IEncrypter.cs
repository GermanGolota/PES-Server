using System.Threading.Tasks;

namespace Infrastructure.Contracts;

public interface IEncrypter
{
    Task<string> Encrypt(string ToBeEncrypt);
}