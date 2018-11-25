namespace AutomationCore.EventBus.Messages.Events
{
    public class LightColorChanged
    {
        public string Color { get; }
        public string LampId { get; }

        public LightColorChanged(string color, string lampId)
        {
            Color = color;
            LampId = lampId;
        }
    }
}
