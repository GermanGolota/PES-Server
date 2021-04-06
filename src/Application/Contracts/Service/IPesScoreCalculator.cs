using Application.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface IPesScoreCalculator
    {
        int CalculateScore(PesScoreModel scoreModel);
    }
}
