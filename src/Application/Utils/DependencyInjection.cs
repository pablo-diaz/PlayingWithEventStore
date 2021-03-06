using System.Reflection;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Application.Utils
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped<EventDispatcher>();

            return services;
        }
    }
}
