using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationCore.EventBus.Messages.Events
{
    public class TrafficChecked
    {
        public int CurrentTravelTime { get; }
        public int DefaultTravelTime { get; }

        public TrafficChecked(int currentTravelTime, int defaultTravelTime)
        {
            CurrentTravelTime = currentTravelTime / 60;
            DefaultTravelTime = defaultTravelTime / 60;
        }
    }
}
