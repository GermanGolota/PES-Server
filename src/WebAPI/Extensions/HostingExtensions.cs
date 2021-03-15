using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Extensions
{
    public static class HostingExtensions
    {
        public static IHost InitializeDBIfNeeded(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                string initialize = config["InitializeDb"];
                bool hasValue = Boolean.TryParse(initialize, out bool shouldInitialize);
                if (hasValue && shouldInitialize)
                {
                    var context = scope.ServiceProvider.GetRequiredService<PESContext>();
                    context.Database.Migrate();
                }
            }

            return host;
        }
    }
}
