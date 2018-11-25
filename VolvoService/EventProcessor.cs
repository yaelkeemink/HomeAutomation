using AutomationCore.EventBus;
using AutomationCore.EventBus.Messages.Commands;
using AutomationCore.EventBus.Messages.Events;
using AutomationCore.Helpers;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Threading.Tasks;
using VolvoService.VolvoTypes.Position;

namespace VolvoService
{
    public class EventProcessor : IEventProcessor
    {
        private readonly ILogger _logger;
        private readonly IEventBusClient _rabbitMQClient;
        private readonly IVolvoClient _volvoClient;
        private readonly string _exchangeName = "HomeAutomationFanout";
        private readonly string _queuename = "VolvoService";

        private DateTime _lastHeatherRun;

        public EventProcessor(ILogger logger, IVolvoClient volvoClient)
        {
            _logger = logger;
            _rabbitMQClient = new RabbitMQClient();
            _volvoClient = volvoClient;
            _lastHeatherRun = DateTime.Now.AddDays(-1);
        }

        public void Run()
        {
            _logger.Information("Setting up RabbitMQ");
            _rabbitMQClient.DeclareExchange(_exchangeName, ExchangeType.Fanout)
                .DeclareQueue(_queuename)
                .BindQueue(_queuename, _exchangeName)
                .Consume<StartHomeAutomation>(_queuename, (message, info) => { Handle(message.Body); })
                .Consume<WeatherChecked>(_queuename, (message, info) => { Handle(message.Body); });
            _logger.Information("All setup");
        }

        private void Handle(WeatherChecked message)
        {
            _logger.Information($"Received a TrafficChecked message{message}");
            Message<HeaterStarted> @event;
            if (message.Temperature < 14 &&
                HeaterRunLastHour())
            {
                AsyncHelper.RunSync(() => _volvoClient.StartHeatherAsync());
                _lastHeatherRun = DateTime.Now;
                _logger.Information($"Heater Started");
                @event = new Message<HeaterStarted>(new HeaterStarted(true, message.Temperature));
            }
            else
            {
                _logger.Information($"No need to start heater: temp = {message.Temperature} last heater start {_lastHeatherRun}");
                @event = new Message<HeaterStarted>(new HeaterStarted(false, message.Temperature));
            }
            _logger.Information("Sending HeaterStarted message");
            _rabbitMQClient.SendMessage(_exchangeName, @event);
            _logger.Information("Message send");
        }

        private void Handle(StartHomeAutomation message)
        {
            _logger.Information($"Received a StartHomeAutomation message{message}");
            RetrieveCurrentPosition();
            RetrieveStatus();
        }

        private async Task RetrieveStatus()
        {
            VolvoTypes.Status.Response status = await _volvoClient.RetrieveStatusAsync();
            _logger.Information("Sending CarLocated message");

            var carStatusChecked = new CarStatusChecked(status.EngineRunning);
            var @event = new Message<CarStatusChecked>(carStatusChecked);

            _rabbitMQClient.SendMessage(_exchangeName, @event);
            _logger.Information("Message send");
        }

        private async Task RetrieveCurrentPosition()
        {
            Position position = await _volvoClient.RetrieveCurrentPositionAsync();

            _logger.Information("Sending CarLocated message");

            var trafficChecked = new CarLocated(position.Longitude, position.Latitude);
            var @event = new Message<CarLocated>(trafficChecked);

            _rabbitMQClient.SendMessage(_exchangeName, @event);
            _logger.Information("Message send");
        }

        private bool HeaterRunLastHour()
        {
            return DateTime.Now.AddHours(-1) > _lastHeatherRun;
        }
    }

    public interface IEventProcessor
    {
        void Run();
    }
}
