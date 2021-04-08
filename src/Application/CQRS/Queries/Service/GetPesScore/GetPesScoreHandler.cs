using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.PesScore;
using Application.DTOs.Service;
using MediatR;

namespace Application.CQRS.Queries.Service.GetPesScore
{
    public class GetPesScoreHandler : IRequestHandler<GetPesScoreQuery, PesScoreResultModel>
    {
        private readonly IPesScoreService _scoreService;
        private readonly IPesScoreCalculator _calculator;
        private readonly IPesScoreConfig _scoreConfig;
        private readonly IPesScoreBadgeLocationResolver _locationResolver;
        private readonly IPesScoreLocalizer _scoreLocalizer;

        public GetPesScoreHandler(IPesScoreService scoreService, IPesScoreCalculator calculator, IPesScoreConfig scoreConfig,
            IPesScoreBadgeLocationResolver locationResolver, IPesScoreLocalizer scoreLocalizer)
        {
            _scoreService = scoreService;
            _calculator = calculator;
            _scoreConfig = scoreConfig;
            _locationResolver = locationResolver;
            _scoreLocalizer = scoreLocalizer;
        }

        public async Task<PesScoreResultModel> Handle(GetPesScoreQuery request, CancellationToken cancellationToken)
        {
            var scoreModel = await _scoreService.GetPesScoreFor(request.Username);
            int score = _calculator.CalculateScore(scoreModel);
            string title = _scoreConfig.GetTitleForScore(score);
            string localizedTitle = _scoreLocalizer.GetLocalizedNameFor(title, CultureInfo.CurrentCulture) ?? title;
            string badgeLocation = _locationResolver.GetLocationOf(title);
            PesScoreResultModel result = new PesScoreResultModel
            {
                BadgeLocation = badgeLocation,
                Title = localizedTitle,
                PesScore = score
            };
            return result;
        }
    }
}
