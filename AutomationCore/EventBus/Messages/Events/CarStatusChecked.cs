using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationCore.EventBus.Messages.Events
{
    public class CarStatusChecked
    {
        public bool EngineRunning { get; }

        public CarStatusChecked(bool engineRunning)
        {
            EngineRunning = engineRunning;
        }
    }
}
