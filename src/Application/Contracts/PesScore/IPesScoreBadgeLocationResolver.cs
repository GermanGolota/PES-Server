namespace Application.Contracts.PesScore;

public interface IPesScoreBadgeLocationResolver
{
    string GetLocationOf(string pesScoreTitle);
}