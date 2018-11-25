using AutomationCore.EventBus;
using AutomationCore.EventBus.Messages.Events;
using AutomationCore.Helpers;
using EasyNetQ.Topology;
using Serilog;

namespace HueService
{
    public class EventProcessor : IEventProcessor
    {
        private readonly ILogger _logger;
        private readonly IEventBusClient _rabbitMQClient;
        private readonly IHueController _hueController;
        private readonly string _exchangeName = "HomeAutomationFanout";
        private readonly string _queuename = "HueService";

        public EventProcessor(ILogger logger, IHueController hueController)
        {
            _logger = logger;
            _rabbitMQClient = new RabbitMQClient();
            _hueController = hueController;
        }

        public void Run()
        {
            _logger.Information("Setting up RabbitMQ");
            _rabbitMQClient.DeclareExchange(_exchangeName, ExchangeType.Fanout)
                .DeclareQueue(_queuename)
                .BindQueue(_queuename, _exchangeName)
                .Consume<TrafficChecked>(_queuename, (message, info) => { Handle(message.Body); });
            _logger.Information("All setup");
        }

        private void Handle(TrafficChecked message)
        {
            _logger.Information($"Current traveltime = {message.CurrentTravelTime}");
            var color = _hueController.GetTrafficColor(message.CurrentTravelTime, message.DefaultTravelTime);
            var changedLight = AsyncHelper.RunSync(() => _hueController.SetLampColorAsync(color));
        }
    }

    public interface IEventProcessor
    {
        void Run();
    }
}
