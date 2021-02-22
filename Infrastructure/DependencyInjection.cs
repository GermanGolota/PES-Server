using Application.Contracts;
using Core;
using Infrastructure.Authentication;
using Infrastructure.Contracts;
using Infrastructure.Repositories;
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

            services.AddScoped<HashAlgorithm>(x=>MD5.Create());
            services.AddScoped<IEncrypter, Encrypter>();
            services.AddScoped<IJWTokenManager, JWTokenManager>();

            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IMessageRepo, MessageRepo>();
            services.AddScoped<IChatRepo, ChatRepo>();

            return services;
        }

        private static string CreateConnectionString(IConfiguration configuration)
        {
            string host = configuration["Host"]??"localhost";
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
