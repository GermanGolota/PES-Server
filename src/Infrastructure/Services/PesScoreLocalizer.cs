using System.Globalization;
using Application.Contracts.PesScore;
using Core.Resources;

namespace Infrastructure.Services;

public class PesScoreLocalizer : IPesScoreLocalizer
{
    public string GetLocalizedNameFor(string pesScoreTitle, CultureInfo culture)
    {
        string result = null;
        if (!string.IsNullOrEmpty(pesScoreTitle)) result = PesTitle.ResourceManager.GetString(pesScoreTitle, culture);
        return result;
    }
}