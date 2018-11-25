namespace AutomationCore.EventBus.Messages.Events
{
    public class HeaterStarted
    {
        public bool HeaterHasStarted { get; }
        public double OutsideTemp { get; }

        public HeaterStarted(bool heaterStarted, double outsideTemp)
        {
            HeaterHasStarted = heaterStarted;
            OutsideTemp = outsideTemp;
        }
    }
}
