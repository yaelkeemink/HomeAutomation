using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationCore.EventBus.Messages.Events
{
    public class WeatherChecked
    {
        public double Temperature { get; }
        public WeatherChecked(double temp)
        {
            Temperature = temp - 273.13;
        }
    }
}
