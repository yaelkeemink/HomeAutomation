using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Serilog;
using System;

namespace HueService
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
                .AddScoped<ILocalHueClient, LocalHueClient>(s =>
                {
                    return new LocalHueClient(config.GetSection("Hue")["ClientIp"]);
                })
                .AddScoped<IHueController, HueController>(s =>
                {
                    var hueConfig = config.GetSection("Hue");
                    var logger = s.GetRequiredService<ILogger>();
                    var hueClient = s.GetRequiredService<ILocalHueClient>();
                    return new HueController(logger, hueConfig, hueClient);
                })
                .AddScoped<IEventProcessor, EventProcessor>();
                
            return services;
        }
    }
}
