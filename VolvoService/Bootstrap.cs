using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using WebRequestHandler;

namespace VolvoService
{
    internal class Bootstrap
    {
        public IServiceCollection RegisterServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder();
            var config = builder.SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            services
                .AddSingleton<ILogger>(new LoggerConfiguration()
                                        .WriteTo.Console()
                                        .WriteTo.RollingFile(config["Logger:LogPath"])
                                        .CreateLogger())
                .AddScoped<IVolvoClient, VolvoClient>(s =>
                {
                    var volvoConfig = config.GetSection("Volvo");
                    var logger = s.GetRequiredService<ILogger>();
                    return new VolvoClient(logger, volvoConfig);
                })
                .AddScoped<IEventProcessor, EventProcessor>();
            return services;
        }
    }
}
