using System.IO;
using Application.Contracts.PesScore;
using Microsoft.AspNetCore.Hosting;

namespace WebAPI.Services;

public class PesScoreBadgeLocationResolver : IPesScoreBadgeLocationResolver
{
    private readonly IWebHostEnvironment _environment;

    public PesScoreBadgeLocationResolver(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string GetLocationOf(string pesScoreTitle)
    {
        string root = _environment.WebRootPath;
        string badgesDirectoryName = "PesBadges";
        string filesLocation = Path.Combine(root, badgesDirectoryName);
        DirectoryInfo dirInfo = new DirectoryInfo(filesLocation);
        FileInfo[] files = dirInfo.GetFiles();
        FileInfo badgeFile = null;
        foreach (FileInfo file in files)
        {
            string nameNoExtension = Path.GetFileNameWithoutExtension(file.Name);
            if (nameNoExtension.Equals(pesScoreTitle))
            {
                badgeFile = file;
                break;
            }
        }

        string output = null;
        if (badgeFile is not null) output = $"/{badgesDirectoryName}/{badgeFile.Name}";
        return output;
    }
}