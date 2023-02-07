using System.Collections.Generic;
using System.Linq;
using Application.Contracts;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class PesScoreConfig : IPesScoreConfig
{
    private readonly IConfiguration _config;
    private readonly Dictionary<int, string> _pesScores;

    public PesScoreConfig(IConfiguration config)
    {
        _config = config;
        _pesScores = _config
            .GetSection("PesConfig")
            .GetChildren()
            .Select(x => new KeyValuePair<int, string>(int.Parse(x.Key), x.Value))
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public string GetTitleForScore(int pesScore)
    {
        List<int> scores = _pesScores.Keys?.ToList();
        int closestScore = GetClosestScore(pesScore, scores);
        string title = _pesScores[closestScore];
        return title;
    }

    private int GetClosestScore(int pesScore, List<int> scores)
    {
        scores.Sort();

        if (pesScore <= scores.First()) return scores.First();

        if (pesScore >= scores.Last()) return scores.Last();

        int closestScore = -1;
        for (int i = 0; i < scores.Count; i++)
        {
            int score = scores[i];
            if (pesScore >= score)
            {
                for (int j = i; j < scores.Count; j++)
                    if (scores[j] < pesScore)
                    {
                        closestScore = scores[j];
                        break;
                    }

                if (closestScore == -1) closestScore = scores.Last();
            }
        }

        if (closestScore == -1) closestScore = scores.First();

        return closestScore;
    }
}