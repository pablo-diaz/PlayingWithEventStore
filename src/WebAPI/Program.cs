using WebAPI.Jobs;

using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((hostingContext, services) => {
                    AddHostedServices(hostingContext.Configuration, services);
                });

        private static void AddHostedServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddHostedService<CertificateEventsSubscription>();
        }
    }
}
