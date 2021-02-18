using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.PipelineBehaviours;

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
    }
}
