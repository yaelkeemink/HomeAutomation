using System;
using AutomationCore.EventBus;
using AutomationCore.EventBus.Messages.Events;
using AutomationCore.Helpers;
using EasyNetQ.Topology;
using Serilog;

namespace NotificationService
{
    public class EventProcessor : IEventProcessor
    {
        private readonly ILogger _logger;
        private readonly IEventBusClient _rabbitMQClient;
        private readonly INotifier _notifier;
        private readonly string _exchangeName = "HomeAutomationFanout";
        private readonly string _queuename = "NotificationService";

        public EventProcessor(ILogger logger, INotifier notifier)
        {
            _logger = logger;
            _rabbitMQClient = new RabbitMQClient();
            _notifier = notifier;
        }

        public void Run()
        {
            _logger.Information("Setting up RabbitMQ");
            _rabbitMQClient.DeclareExchange(_exchangeName, ExchangeType.Fanout)
                .DeclareQueue(_queuename)
                .BindQueue(_queuename, _exchangeName)
                .Consume<TrafficChecked>(_queuename, (message, info) => { Handle(message.Body); })
                .Consume<HeaterStarted>(_queuename, (message, info) => { Handle(message.Body); });
            _logger.Information("All setup");
        }

        private void Handle(HeaterStarted body)
        {
            string message = $"Car is heating up right now. It will be ready in 15 minutes. Outside temperature is {body.OutsideTemp} degrees";
            if (!body.HeaterHasStarted)
            {
                message = $"no need to turn on the car heater. It is {body.OutsideTemp} degrees outside.";
            }
            AsyncHelper.RunSync(() => _notifier.PostMessageAsync(message));
        }

        private void Handle(TrafficChecked body)
        {
            string state = "no need to rush";
            if (body.CurrentTravelTime > body.DefaultTravelTime + 5)
            {
                state = "you should leave now";
            }
            var message = $"Current traveltime: {body.CurrentTravelTime} minutes, {state}";
            AsyncHelper.RunSync(() => _notifier.PostMessageAsync(message));
        }
    }

    public interface IEventProcessor
    {
        void Run();
    }
}
