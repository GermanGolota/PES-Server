using Application.Contracts.PesScore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Services
{
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
            var dirInfo = new DirectoryInfo(filesLocation);
            var files = dirInfo.GetFiles();
            FileInfo badgeFile = null;
            foreach (var file in files)
            {
                string nameNoExtension = Path.GetFileNameWithoutExtension(file.Name);
                if(nameNoExtension.Equals(pesScoreTitle))
                {
                    badgeFile = file;
                    break;
                }
            }

            string output = null;
            if(badgeFile is not null)
            {
                output = Path.Combine(badgesDirectoryName, badgeFile.Name);
            }
            return output;
        }
    }
}
