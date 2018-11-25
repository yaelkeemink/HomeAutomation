using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using AutomationCore.EventBus;
using EasyNetQ.Topology;
using EasyNetQ;
using AutomationCore.EventBus.Messages.Commands;
using AutomationCore.EventBus.Messages.Events;

namespace AutomationController
{
    public interface IAutomation
    {
        void Start();
    }
    public class Automation : IAutomation
    {
        private readonly IConfigurationSection _config;
        private readonly IEventBusClient _rabbitMQClient;
        private readonly ILogger _logger;
    
        private DateTime _currentTime = DateTime.Now;
        private readonly string _exchangeName = "HomeAutomationFanout";
        private readonly string _queuename = "AutomationController";

        private TimeSpan _checkDelay;

        public Automation(ILogger logger, 
            IConfigurationSection config)
        {
            _logger = logger;
            _config = config;
            _rabbitMQClient = new RabbitMQClient()
                .DeclareExchange(_exchangeName, ExchangeType.Fanout)
                .DeclareQueue(_queuename)
                .BindQueue(_queuename, _exchangeName)
                .Consume<CarStatusChecked>(_queuename, (message, info) => { Handle(message.Body); });                
        }

        private void Handle(CarStatusChecked body)
        {
            if (body.EngineRunning)
            {
                _checkDelay = new TimeSpan(8, 45, 0);
            }
        }

        public void Start()
        {            
            _logger.Information("Starting app");
            var lastChecked = _currentTime.AddMinutes(-6);

            while (true)
            {
                _logger.Information($"Check every {_checkDelay} minutes");
                _checkDelay = GetTimeSpanFromString(_config["CheckDelay"]);
                if (_currentTime.DayOfWeek != DayOfWeek.Sunday &&
                    _currentTime.DayOfWeek != DayOfWeek.Saturday &&
                    ActiveMorningHours(_currentTime.TimeOfDay) &&
                    lastChecked.AddMinutes(5) < _currentTime)
                {
                    _logger.Information($"Now checking");
                    var message = new Message<StartHomeAutomation>(new StartHomeAutomation());
                    _rabbitMQClient.SendMessage(_exchangeName, message);
                }
                else
                {
                    _logger.Information("No need to check, checking again in 5 minutes");
                }
                Thread.Sleep(_checkDelay);
                _currentTime = DateTime.Now;
            }
        }

        private TimeSpan GetTimeSpanFromString(string time)
        {
            var timeArray = time.Split(":")
                .Select(a => int.Parse(a))
                .ToArray();
            return new TimeSpan(timeArray[0], timeArray[1], 0);
        }

        private bool ActiveMorningHours(TimeSpan currentTime)
        {            
            return currentTime > GetTimeSpanFromString(_config["MorningStart"]) &&
                    currentTime < GetTimeSpanFromString(_config["MorningEnd"]);
        }
    }
}
