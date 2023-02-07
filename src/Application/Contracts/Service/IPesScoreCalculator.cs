using Application.DTOs.Service;

namespace Application.Contracts;

public interface IPesScoreCalculator
{
    int CalculateScore(PesScoreModel scoreModel);
}