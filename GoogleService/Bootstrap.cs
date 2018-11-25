using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using WebRequestHandler;

namespace GoogleService
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
                .AddScoped<IGoogleController, GoogleController>(s =>
                {
                    var googleConfig = config.GetSection("Google");
                    var httpClientHandler = s.GetRequiredService<IHttpClientHandler>();
                    var logger = s.GetRequiredService<ILogger>();
                    return new GoogleController(logger, httpClientHandler, googleConfig);
                })
                .AddScoped<IEventProcessor, EventProcessor>();
            return services;
        }
    }
}
