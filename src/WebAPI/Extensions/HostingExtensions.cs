using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebAPI.Extensions;

public static class HostingExtensions
{
    public static IHost InitializeDBIfNeeded(this IHost host)
    {
        using (IServiceScope scope = host.Services.CreateScope())
        {
            IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string initialize = config["InitializeDb"];
            bool hasValue = bool.TryParse(initialize, out bool shouldInitialize);
            if (hasValue && shouldInitialize)
            {
                PESContext context = scope.ServiceProvider.GetRequiredService<PESContext>();
                context.Database.Migrate();
            }
        }

        return host;
    }
}