using AutomationCore.EventBus;
using AutomationCore.EventBus.Messages.Commands;
using AutomationCore.EventBus.Messages.Events;
using AutomationCore.Helpers;
using EasyNetQ;
using EasyNetQ.Topology;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Threading.Tasks;

namespace GoogleService
{
    public class EventProcessor : IEventProcessor
    {
        private readonly ILogger _logger;
        private readonly IEventBusClient _rabbitMQClient;
        private readonly IGoogleController _googleController;
        private readonly string _exchangeName = "HomeAutomationFanout";
        private readonly string _queuename = "GoogleService";

        public EventProcessor(ILogger logger, IGoogleController googleController)
        {
            _logger = logger;
            _rabbitMQClient = new RabbitMQClient();
            _googleController = googleController;
        }

        public void Run()
        {
            _logger.Information("Setting up RabbitMQ");
            _rabbitMQClient.DeclareExchange(_exchangeName, ExchangeType.Fanout)
                .DeclareQueue(_queuename)
                .BindQueue(_queuename, _exchangeName)
                .Consume<CarLocated>(_queuename, (message, info) => { Handle(message.Body); });
            _logger.Information("All setup");
        }

        

        private void Handle(CarLocated message)
        {
            CheckTraffic(message);
            CheckWeather(message);
        }

        private async Task CheckTraffic(CarLocated message)
        {
            _logger.Information($"received a CheckTraffic message{message}");
            JObject trafficCheckedjson = await _googleController.GetCurrentTravelTime(message.Latitude, message.Longitude);

            _logger.Information("Sending TrafficChecked message");
            var currentTrafficTime = (int)trafficCheckedjson["routes"][0]["legs"][0]["duration_in_traffic"]["value"];
            var defaultTrafficTime = (int)trafficCheckedjson["routes"][0]["legs"][0]["duration"]["value"];

            var trafficChecked = new TrafficChecked(currentTrafficTime, defaultTrafficTime);
            var @event = new Message<TrafficChecked>(trafficChecked);
            _rabbitMQClient.SendMessage(_exchangeName, @event);
            _logger.Information("Message send");
        }

        private async Task CheckWeather(CarLocated message)
        {
            _logger.Information($"received a WeatherChecked message{message}");
            JObject weatherCheckedJson = await _googleController.GetCurrentWeather(message.Latitude, message.Longitude);

            _logger.Information("Sending WeatherChecked message");
            var weatherChecked = new WeatherChecked((double)weatherCheckedJson["main"][0]["temp"]);
            var @event = new Message<WeatherChecked>(weatherChecked);

            _rabbitMQClient.SendMessage(_exchangeName, @event);
            _logger.Information("Message send");
        }
    }

    public interface IEventProcessor
    {
        void Run();
    }
}
