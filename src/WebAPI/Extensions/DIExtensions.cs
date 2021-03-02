using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using WebAPI.PipelineBehaviours;
using WebAPI.WebSockets;

namespace WebAPI.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBahavior<,>));
            return services;
        }

        public static IServiceCollection AddJwtTokenAuthorization(this IServiceCollection services,
            IConfiguration conf)
        {
            string key = conf["EncryptionKey"];
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }

        public static IServiceCollection AddWebsocketServices(this IServiceCollection services,
            IConfiguration config)
        {
            string client = config.GetValue<string>("WebSocketClient");
            services.AddWebSockets(x =>
            {
                x.AllowedOrigins.Add(client);
                x.KeepAliveInterval = TimeSpan.FromSeconds(30);
            }
            );
            services.AddSingleton<IWebSocketsManager, WebSocketsManager>();
            return services;
        }
    }
}
