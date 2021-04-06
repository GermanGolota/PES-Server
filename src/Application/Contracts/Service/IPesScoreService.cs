using Application.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IPesScoreService
    {
        Task<PesScoreModel> GetPESScoreFor(string username);
        Task<PesScoreModel> GetPESScoreFor(Guid userId);
    }
}
