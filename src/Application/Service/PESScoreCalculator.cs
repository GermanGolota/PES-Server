using Application.Contracts;
using Application.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.PesScore
{
    public class PesScoreCalculator : IPesScoreCalculator
    {
        public int CalculateScore(PesScoreModel scoreModel)
        {
            List<char> pesChars = GetAllLettersWithCounterparts(scoreModel.PesKey);
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

        private List<char> GetAllLettersWithCounterparts(string str)
        {
            var chars = str.ToCharArray();
            List<char> output = chars.ToList();
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                if(Char.IsLetter(ch))
                {
                    if(Char.IsLower(ch))
                    {
                        output.Add(Char.ToUpper(ch));
                    }
                    else
                    {
                        if(Char.IsUpper(ch))
                        {
                            output.Add(Char.ToLower(ch));
                        }
                    }
                }
            }
            return output;
        }
    }
}
