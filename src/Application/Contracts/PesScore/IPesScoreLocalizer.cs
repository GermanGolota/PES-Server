using System.Globalization;

namespace Application.Contracts.PesScore;

public interface IPesScoreLocalizer
{
    string GetLocalizedNameFor(string pesScoreTitle, CultureInfo culture);
}