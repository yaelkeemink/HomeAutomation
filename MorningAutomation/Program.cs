using Microsoft.Extensions.DependencyInjection;

namespace AutomationController
{
    public class Program
    {

        static void Main(string[] args)
        {
            var serviceProvider = new Bootstrap()
                .RegisterServices(new ServiceCollection())
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var automation = scope.ServiceProvider.GetService<IAtomation>();
                automation.Start();
            }
        }
    }
}
