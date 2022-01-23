using Application.Utils;

using Infrastructure.ConfigDTOs;
using Infrastructure.Persistence;

using EventStore.Client;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Utils
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton(sp => {
                var eventStoreDBOptions = sp.GetRequiredService<IOptions<EventStoreDBOptions>>().Value;
                return new EventStoreClient(EventStoreClientSettings.Create(eventStoreDBOptions.ConnectionString));
            });

            services.AddScoped<EventsStore, EventsStoreDBImpl>();

            return services;
        }
    }
}
