using Application.Contracts.PesScore;
using Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Infrastructure.Services
{
    public class PesScoreLocalizer : IPesScoreLocalizer
    {
        public string GetLocalizedNameFor(string pesScoreTitle, CultureInfo culture)
        {
            string result = null;
            if(!String.IsNullOrEmpty(pesScoreTitle))
            {
                result = PesTitle.ResourceManager.GetString(pesScoreTitle, culture);
            }
            return result;
        }
    }
}
