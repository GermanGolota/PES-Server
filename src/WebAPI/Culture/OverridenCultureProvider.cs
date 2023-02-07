using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;

namespace WebAPI.Culture;

public class OverridenCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        ProviderCultureResult result = null;
        IHeaderDictionary headers = httpContext.Request.Headers;
        string overrideKey = "User-Culture-Override";
        if (headers.ContainsKey(overrideKey))
        {
            bool wasSuccessfull = headers.TryGetValue(overrideKey, out StringValues values);
            if (wasSuccessfull)
            {
                string culture = values.FirstOrDefault();
                if (!string.IsNullOrEmpty(culture))
                {
                    StringSegment cultureSegment = new StringSegment(culture);
                    result = new ProviderCultureResult(cultureSegment);
                }
            }
        }

        return Task.FromResult(result);
    }
}