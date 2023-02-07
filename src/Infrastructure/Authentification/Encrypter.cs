using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Contracts;

namespace Infrastructure.Authentication;

public class Encrypter : IEncrypter
{
    private readonly HashAlgorithm _algorithm;

    public Encrypter(HashAlgorithm algorithm)
    {
        _algorithm = algorithm;
    }

    public Task<string> Encrypt(string ToBeEncrypt)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(ToBeEncrypt);
        byte[] hash = _algorithm.ComputeHash(bytes);
        string output = Convert.ToBase64String(hash);
        return Task.FromResult(output);
    }
}