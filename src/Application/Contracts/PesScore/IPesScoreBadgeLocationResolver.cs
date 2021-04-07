using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts.PesScore
{
    public interface IPesScoreBadgeLocationResolver
    {
        string GetLocationOf(string pesScoreTitle);
    }
}
