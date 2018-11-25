using Microsoft.Extensions.DependencyInjection;
using System;

namespace NotificationService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var serviceProvider = new Bootstrap()
                .RegisterServices(new ServiceCollection())
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var eventProcessor = scope.ServiceProvider.GetService<IEventProcessor>();
                eventProcessor.Run();
            }
        }
    }
}
