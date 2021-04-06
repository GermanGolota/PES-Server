using System.Collections.Generic;
using System.Linq;

namespace Application.PESScore
{
    public class PESScoreCalculator
    {
        public int CalculateScore(List<string> messages, string pesKey)
        {
            List<char> pesChars = pesKey.ToList();
            long matches = 0;
            long total = 0;
            foreach (string message in messages)
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
