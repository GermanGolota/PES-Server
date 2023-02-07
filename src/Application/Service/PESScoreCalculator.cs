using System.Collections.Generic;
using System.Linq;
using Application.Contracts;
using Application.DTOs.Service;
using Core.Extensions;

namespace Application.PesScore;

public class PesScoreCalculator : IPesScoreCalculator
{
    public int CalculateScore(PesScoreModel scoreModel)
    {
        int result = 0;
        if (scoreModel.PesKey.IsFilled() && scoreModel.Messages.IsNotNullOrEmpty())
        {
            List<char> pesChars = GetAllLettersWithCounterparts(scoreModel.PesKey);
            long matches = 0;
            long total = 0;
            foreach (string message in scoreModel.Messages)
                for (int i = 0; i < message.Length; i++)
                {
                    total++;
                    char ch = message[i];
                    if (pesChars.Contains(ch)) matches++;
                }

            if (total != 0) result = (int)(100 * matches / total);
        }

        return result;
    }

    private List<char> GetAllLettersWithCounterparts(string str)
    {
        char[] chars = str.ToCharArray();
        List<char> output = chars.ToList();
        for (int i = 0; i < chars.Length; i++)
        {
            char ch = chars[i];
            if (char.IsLetter(ch))
            {
                if (char.IsLower(ch))
                {
                    output.Add(char.ToUpper(ch));
                }
                else
                {
                    if (char.IsUpper(ch)) output.Add(char.ToLower(ch));
                }
            }
        }

        return output;
    }
}