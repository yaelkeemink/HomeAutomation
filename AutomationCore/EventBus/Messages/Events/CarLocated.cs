namespace AutomationCore.EventBus.Messages.Events
{
    public class CarLocated
    {
        public double Longitude { get; }
        public double Latitude { get; }

        public CarLocated(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}