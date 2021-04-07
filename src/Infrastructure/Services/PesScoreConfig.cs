using Application.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Services
{
    public class PesScoreConfig : IPesScoreConfig
    {
        private readonly IConfiguration _config;
        private readonly Dictionary<int, string> _pesScores;
        public PesScoreConfig(IConfiguration config)
        {
            _config = config;
            _pesScores = _config.GetSection("PesConfig")
                .GetChildren()
                .Select(x => new KeyValuePair<int, string>(Int32.Parse(x.Key), x.Value))
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public string GetTitleForScore(int pesScore)
        {
            var scores = _pesScores.Keys?.ToList();
            int closestScore = GetClosestScore(pesScore, scores);
            string title = _pesScores[closestScore];
            return title;
        }

        private int GetClosestScore(int pesScore, List<int> scores)
        {
            int closestScore = -1;
            for (int i = 0; i < scores.Count; i++)
            {
                int score = scores[i];
                if (pesScore >= score)
                {
                    if (NotLastScore(scores, i))
                    {
                        closestScore = score;
                        break;
                    }
                    else
                    {
                        closestScore = scores.Last();
                        break;
                    }
                }
            }

            if (closestScore != -1)
            {
                closestScore = scores.First();
            }

            return closestScore;
        }

        private bool NotLastScore(List<int> scores, int i)
        {
            return i != scores.Count - 1;
        }
    }
}
