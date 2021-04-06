using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using MediatR;

namespace Application.CQRS.Queries.Service.GetPesScore
{
    public class GetPesScoreHandler : IRequestHandler<GetPesScoreQuery, int>
    {
        private readonly IPesScoreService _scoreService;
        private readonly IPesScoreCalculator _calculator;

        public GetPesScoreHandler(IPesScoreService scoreService, IPesScoreCalculator calculator)
        {
            _scoreService = scoreService;
            _calculator = calculator;
        }
        public async Task<int> Handle(GetPesScoreQuery request, CancellationToken cancellationToken)
        {
            var scoreModel = await _scoreService.GetPesScoreFor(request.Username);
            int score = _calculator.CalculateScore(scoreModel);
            return score;
        }
    }
}
