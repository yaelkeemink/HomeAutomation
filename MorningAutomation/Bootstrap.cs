using Hue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using WebRequestHandler;
using WebRequestHandler.Google;

namespace MorningAutomation
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
                .AddScoped<IHttpClientHandler, HttpClientHandler>()
                .AddSingleton<INotifier, Notifier>((s) =>
                {
                    var httpClientHandler = s.GetRequiredService<IHttpClientHandler>();
                    var logger = s.GetRequiredService<ILogger>();
                    return new Notifier(logger, httpClientHandler, config["ConnectionStrings:SlackConnectionString"]);
                })
                .AddScoped<IHueController, HueController>(s =>
                {
                    var hueConfig = config.GetSection("Hue");
                    var logger = s.GetRequiredService<ILogger>();
                    return new HueController(logger, hueConfig["ClientIp"], hueConfig["UserName"]);
                })                
                .AddScoped<IGoogleController, GoogleController>(s =>
                {
                    var googleConfig = config.GetSection("Google");
                    var httpClientHandler = s.GetRequiredService<IHttpClientHandler>();
                    var logger = s.GetRequiredService<ILogger>();
                    return new GoogleController(logger, httpClientHandler, googleConfig["BaseUri"], googleConfig["ApiKey"]);
                })
                .AddScoped<IAtomation, Automation>(s =>
                {
                    var automationConfig = config.GetSection("Automation");
                    var logger = s.GetRequiredService<ILogger>();
                    return new Automation(logger, 
                        s.GetRequiredService<IGoogleController>(),
                        s.GetRequiredService<IHueController>(),
                        s.GetRequiredService<INotifier>(),
                        automationConfig);
                });
            return services;
        }
    }
}
