using Hue;
using Microsoft.Extensions.Configuration;
using AutomationController.Helpers;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using WebRequestHandler;
using GoogleService;

namespace AutomationController
{
    public interface IAtomation
    {
        void Start();
    }
    public class Automation : IAtomation
    {
        private readonly IConfigurationSection _config;
        private readonly ILogger _logger;
        private readonly IGoogleController _googleController;
        private readonly IHueController _hueController;
        private readonly INotifier _notifier;
        private DateTime currentTime = DateTime.Now;
        public Automation(ILogger logger, 
            IGoogleController googleController, 
            IHueController hueController, 
            INotifier notifier, 
            IConfigurationSection config)
        {
            _logger = logger;
            _googleController = googleController;
            _hueController = hueController;
            _notifier = notifier;
            _config = config;
        }
        public void Start()
        {
            string from = _config["FromLocation"];
            string to = _config["ToLocation"];

            _logger.Information("Starting app");
            _logger.Information("Check every 5 minutes");
            _logger.Information($"From {_config["MorningStart"]} to {_config["MorningEnd"]}");
            _logger.Information($"And from {_config["NoonStart"]} to {_config["NoonEnd"]}");
            _logger.Information($"FromLocation: {from}");
            _logger.Information($"ToLoaction: {to}");
            _logger.Information($"LightId: {_config["MainLight"]}");

            
            var lastChecked = currentTime.AddMinutes(-6);

            while (true)
            {
                if (currentTime.DayOfWeek != DayOfWeek.Sunday &&
                    currentTime.DayOfWeek != DayOfWeek.Saturday &&
                    ActiveHours(currentTime.TimeOfDay) &&
                    lastChecked.AddMinutes(5) < currentTime)
                {
                    _logger.Information($"Now checking");
                    int travelTime;
                    if (ActiveMorningHours(currentTime.TimeOfDay))
                    {
                        travelTime = AsyncHelper.RunSync(() => _googleController.GetCurrentTravelTime(from, to));
                        _logger.Information($"Current travelTime: {travelTime}");
                    }
                    else
                    {
                        travelTime = AsyncHelper.RunSync(() => _googleController.GetCurrentTravelTime(to, from));
                        _logger.Information($"Current travelTime: {travelTime}");
                    }
                    CheckTravelTime(travelTime, GetSecondsFromMinutes(50));
                    
                }
                else
                {
                    _logger.Information("No need to check, checking again in 5 minutes");
                }
                Thread.Sleep(GetTimeSpanFromString(_config["CheckDelay"]));
                currentTime = DateTime.Now;
            }
        }

        private TimeSpan GetTimeSpanFromString(string time)
        {
            var timeArray = time.Split(":")
                .Select(a => int.Parse(a))
                .ToArray();
            return new TimeSpan(timeArray[0], timeArray[1], 0);
        }

        private void CheckTravelTime(int currentTravelTime, int defaultTravelTime)
        {
            //White
            var color = "FFFFFF";
            var state = "No traffic";
            if (currentTravelTime > defaultTravelTime + GetSecondsFromMinutes(5))
            {
                //Orange
                color = "FFA500";
                state = "Little delay, leave soon";
            }
            else if (currentTravelTime > defaultTravelTime + GetSecondsFromMinutes(10))
            {
                //Red
                color = "FF0000";
                state = "Much delay, leave now";
            }

            string lightId = _config["MainLight"];
            if (ActiveMorningHours(currentTime.TimeOfDay))
            {
                _hueController.SetLampColorAsync(lightId, color);
            }
            AsyncHelper.RunSync(() => _notifier.PostMessageAsync(GetMinutesFromSeconds(currentTravelTime), state));
        }

        private int GetSecondsFromMinutes(int minutes)
        {
            return minutes * 60;
        }

        private int GetMinutesFromSeconds(int minutes)
        {
            return minutes / 60;
        }

        private bool ActiveHours(TimeSpan currentTime)
        {
            return ActiveMorningHours(currentTime) || ActiveNoonHours(currentTime);
        }

        private bool ActiveNoonHours(TimeSpan currentTime)
        {
            return currentTime > GetTimeSpanFromString(_config["NoonStart"]) &&
                    currentTime < GetTimeSpanFromString(_config["NoonEnd"]);
        }

        private bool ActiveMorningHours(TimeSpan currentTime)
        {            
            return currentTime > GetTimeSpanFromString(_config["MorningStart"]) &&
                    currentTime < GetTimeSpanFromString(_config["MorningEnd"]);
        }
    }
}
