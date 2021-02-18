using Application.Contracts;
using Core;
using Infrastructure.Authentication;
using Infrastructure.Contracts;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Cryptography;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PESContext>(x =>
                {
                    x.UseSqlServer(configuration.GetConnectionString("Main"), b =>
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
    }
}
