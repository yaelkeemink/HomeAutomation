using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace AutomationController
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
                .AddScoped<IAutomation, Automation>(s =>
                {
                    var automationConfig = config.GetSection("Automation");
                    var logger = s.GetRequiredService<ILogger>();
                    return new Automation(logger, automationConfig);
                });
            return services;
        }
    }
}
