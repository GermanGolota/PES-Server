using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Culture
{
    public class OverridenCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            ProviderCultureResult result = null;
            var headers = httpContext.Request.Headers;
            string overrideKey = "User-Culture-Override";
            if (headers.ContainsKey(overrideKey))
            {
                bool wasSuccessfull = headers.TryGetValue(overrideKey, out StringValues values);
                if(wasSuccessfull)
                {
                    string culture = values.FirstOrDefault();
                    if (!String.IsNullOrEmpty(culture))
                    {
                        var cultureSegment = new StringSegment(culture);
                        result = new ProviderCultureResult(cultureSegment);
                    }
                }
            }
            return result;
        }
    }
}
