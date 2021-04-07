using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Application.Contracts.PesScore
{
    public interface IPesScoreLocalizer
    {
        string GetLocalizedNameFor(string pesScoreTitle, CultureInfo culture);
    }
}
