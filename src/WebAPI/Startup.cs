using Application.Utils;

using Infrastructure.Utils;
using Infrastructure.ConfigDTOs;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AddConfigurationOptions(services);

            services.AddControllers();

            services.AddApplication();
            services.AddInfrastructure();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

        private void AddConfigurationOptions(IServiceCollection services)
        {
            services.AddOptions<EventStoreDBOptions>().Bind(Configuration.GetSection("EventStoreDbConfig"));
            services.AddOptions<ElasticSearchOptions>().Bind(Configuration.GetSection("ElasticSearchConfig"));
        }
    }
}
