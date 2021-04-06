using Application.Contracts;
using Application.DTOs.Service;
using System.Collections.Generic;
using System.Linq;

namespace Application.PesScore
{
    public class PESScoreCalculator : IPesScoreCalculator
    {
        public int CalculateScore(PesScoreModel scoreModel)
        {
            List<char> pesChars = scoreModel.PesKey.ToList();
            long matches = 0;
            long total = 0;
            foreach (string message in scoreModel.Messages)
            {
                for (int i = 0; i < message.Length; i++)
                {
                    total++;
                    char ch = message[i];
                    if (pesChars.Contains(ch))
                    {
                        matches++;
                    }
                }
            }
            return 100 * (int)(matches / total);
        }
    }
}
