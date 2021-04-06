using Application.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IPesScoreService
    {
        Task<PesScoreModel> GetPesScoreFor(string username);
        Task<PesScoreModel> GetPesScoreFor(Guid userId);
    }
}
