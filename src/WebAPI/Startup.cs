using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure;
using WebAPI.Extensions;
using Microsoft.AspNetCore.Localization;
using WebAPI.Culture;
using System.Collections.Generic;
using System.Globalization;
using Application.Contracts.PesScore;
using WebAPI.Services;
using Application.Contracts.Service;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructureServices(Configuration);

            services.AddValidators();

            services.AddAuthentication();

            services.AddJwtTokenAuthorization(Configuration);

            services.AddControllers();

            services.AddSwaggerWithAuthorization();

            services.AddWebsocketServices();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });

            services.AddCustomLocalization();

            services.AddScoped<IPesScoreBadgeLocationResolver, PesScoreBadgeLocationResolver>();
            services.AddScoped<IChatImageService, ChatImageService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PES WebAPI v1"));
            }

            app.UseStaticFiles();

            app.UseExceptionHandler(handler => handler.UseCustomErrors(env));

            app.UseWebSocketsServer();

            app.UseAuthentication();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
