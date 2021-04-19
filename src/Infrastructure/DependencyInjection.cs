using Application.Contracts;
using Application.Contracts.PesScore;
using Application.Contracts.Repositories;
using Application.Contracts.Service;
using Application.PesScore;
using Core;
using Infrastructure.Authentication;
using Infrastructure.Config;
using Infrastructure.Contracts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.WebSockets;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Security.Cryptography;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = CreateConnectionString(configuration);

            services.AddDbContext<PESContext>(x =>
                {
                    x.UseNpgsql(connectionString, b =>
                    {
                        b.MigrationsAssembly(nameof(Infrastructure));
                    });
                }
            );

            Assembly applicationAssembly = typeof(IChatRepo).Assembly;
            services.AddMediatR(applicationAssembly);

            services.AddSecurityServices();

            services.AddRepositories();

            services.AddWebSocketServices();

            services.AddPesScoreServices();

            services.AddScoped<TokenConfig>(factory => new TokenConfig
            {
                EncryptionKey = configuration["EncryptionKey"]
            });

            services.AddScoped<IChatMembersService, ChatMembersService>();

            return services;
        }
        private static IServiceCollection AddSecurityServices(this IServiceCollection services)
        {
            services.AddScoped<HashAlgorithm>(x => MD5.Create());
            services.AddScoped<IEncrypter, Encrypter>();
            services.AddScoped<IJWTokenManager, JWTokenManager>();

            return services;
        }

        private static IServiceCollection AddPesScoreServices(this IServiceCollection services)
        {
            services.AddScoped<IPesScoreCalculator, PesScoreCalculator>();
            services.AddScoped<IPesScoreService, PesScoreService>();
            services.AddScoped<IPesScoreConfig, PesScoreConfig>();
            services.AddScoped<IPesScoreLocalizer, PesScoreLocalizer>();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IMessageRepo, MessageRepo>();
            services.AddScoped<IChatRepo, ChatRepo>();

            return services;
        }

        private static IServiceCollection AddWebSocketServices(this IServiceCollection services)
        {
            services.AddScoped<IMessageSender, MessageSender>();

            return services;
        }

        private static string CreateConnectionString(IConfiguration configuration)
        {
            string host = configuration["Host"] ?? "localhost";
            string port = configuration["Port"] ?? "5432";
            string database = configuration["DB"] ?? "PesDB";
            string username = configuration["User"] ?? "postgres";
            string password = configuration["Password"] ?? throw new Exception("Please " +
                "set a password field");
            string output = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
            return output;
        }
    }
}
