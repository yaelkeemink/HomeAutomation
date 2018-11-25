using Microsoft.Extensions.DependencyInjection;

namespace GoogleService
{
    class Program
    {
        static void Main(string[] args)
        {
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
