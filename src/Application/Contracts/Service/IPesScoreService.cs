using System;
using System.Threading.Tasks;
using Application.DTOs.Service;

namespace Application.Contracts;

public interface IPesScoreService
{
    Task<PesScoreModel> GetPesScoreFor(string username);
    Task<PesScoreModel> GetPesScoreFor(Guid userId);
}