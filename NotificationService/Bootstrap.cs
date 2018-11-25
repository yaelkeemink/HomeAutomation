using AutomationCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService;
using Serilog;
using System;
using WebRequestHandler;

namespace NotificationService
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
                .AddScoped<INotifier, Notifier>(s =>
                {
                    var logger = s.GetRequiredService<ILogger>();
                    var httpClient = s.GetRequiredService<IHttpClientHandler>(); 
                    return new Notifier(logger, httpClient, config.GetSection("ConnectionStrings")["SlackConnectionString"]);
                })
                .AddScoped<IEventProcessor, EventProcessor>();
            return services;
        }
    }
}
