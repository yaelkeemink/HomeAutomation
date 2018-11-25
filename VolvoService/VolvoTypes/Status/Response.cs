using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace VolvoService.VolvoTypes.Status
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "averageFuelConsumption")]
        public double AverageFuelConsumption { get; set; }

        [DataMember(Name = "averageFuelConsumptionTimestamp")]
        public string AverageFuelConsumptionTimestamp { get; set; }

        [DataMember(Name = "averageSpeed")]
        public int AverageSpeed { get; set; }

        [DataMember(Name = "averageSpeedTimestamp")]
        public string AverageSpeedTimestamp { get; set; }

        [DataMember(Name = "brakeFluid")]
        public string BrakeFluid { get; set; }

        [DataMember(Name = "brakeFluidTimestamp")]
        public string BrakeFluidTimestamp { get; set; }

        [DataMember(Name = "carLocked")]
        public bool CarLocked { get; set; }

        [DataMember(Name = "carLockedTimestamp")]
        public string CarLockedTimestamp { get; set; }

        [DataMember(Name = "distanceToEmpty")]
        public int DistanceToEmpty { get; set; }

        [DataMember(Name = "distanceToEmptyTimestamp")]
        public string DistanceToEmptyTimestamp { get; set; }

        [DataMember(Name = "doors")]
        public Doors Doors { get; set; }

        [DataMember(Name = "engineRunning")]
        public bool EngineRunning { get; set; }

        [DataMember(Name = "engineRunningTimestamp")]
        public string EngineRunningTimestamp { get; set; }

        [DataMember(Name = "fuelAmount")]
        public int FuelAmount { get; set; }

        [DataMember(Name = "fuelAmountLevel")]
        public int FuelAmountLevel { get; set; }

        [DataMember(Name = "fuelAmountLevelTimestamp")]
        public string FuelAmountLevelTimestamp { get; set; }

        [DataMember(Name = "fuelAmountTimestamp")]
        public string FuelAmountTimestamp { get; set; }

        [DataMember(Name = "heater")]
        public Heater Heater { get; set; }

        [DataMember(Name = "odometer")]
        public int Odometer { get; set; }

        [DataMember(Name = "odometerTimestamp")]
        public string OdometerTimestamp { get; set; }

        [DataMember(Name = "serviceWarningStatus")]
        public string ServiceWarningStatus { get; set; }

        [DataMember(Name = "serviceWarningStatusTimestamp")]
        public string ServiceWarningStatusTimestamp { get; set; }

        [DataMember(Name = "theftAlarm")]
        public string TheftAlarm { get; set; }

        [DataMember(Name = "timeFullyAccessibleUntil")]
        public string TimeFullyAccessibleUntil { get; set; }

        [DataMember(Name = "timePartiallyAccessibleUntil")]
        public string TimePartiallyAccessibleUntil { get; set; }

        [DataMember(Name = "tripMeter1")]
        public int TripMeter1 { get; set; }

        [DataMember(Name = "tripMeter1Timestamp")]
        public string TripMeter1Timestamp { get; set; }

        [DataMember(Name = "tripMeter2")]
        public int TripMeter2 { get; set; }

        [DataMember(Name = "tripMeter2Timestamp")]
        public string TripMeter2Timestamp { get; set; }

        [DataMember(Name = "tyrePressure")]
        public TyrePressure TyrePressure { get; set; }

        [DataMember(Name = "washerFluidLevel")]
        public string WasherFluidLevel { get; set; }

        [DataMember(Name = "washerFluidLevelTimestamp")]
        public string WasherFluidLevelTimestamp { get; set; }

        [DataMember(Name = "windows")]
        public Windows Windows { get; set; }
    }

    [DataContract]
    public class Windows
    {
        [DataMember(Name = "frontLeftWindowOpen")]
        public bool FrontLeftWindowOpen { get; set; }

        [DataMember(Name = "frontRightWindowOpen")]
        public bool FrontRightWindowOpen { get; set; }

        [DataMember(Name = "rearRightWindowOpen")]
        public bool RearRightWindowOpen { get; set; }

        [DataMember(Name = "rearLeftWindowOpen")]
        public bool RearLeftWindowOpen { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }
    }

    [DataContract]
    public class Doors
    {
        [DataMember(Name = "tailgateOpen")]
        public bool TailgateOpen { get; set; }

        [DataMember(Name = "rearRightDoorOpen")]
        public bool RearRightDoorOpen { get; set; }

        [DataMember(Name = "rearLeftDoorOpen")]
        public bool RearLeftDoorOpen { get; set; }

        [DataMember(Name = "frontRightDoorOpen")]
        public bool FrontRightDoorOpen { get; set; }

        [DataMember(Name = "frontLeftDoorOpen")]
        public bool FrontLeftDoorOpen { get; set; }

        [DataMember(Name = "hoodOpen")]
        public bool HoodOpen { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }
    }

    [DataContract]
    public class Heater
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "timer1")]
        public Timer Timer1 { get; set; }

        [DataMember(Name = "timer2")]
        public Timer Timer2 { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }
    }

    [DataContract]
    public class Timer
    {
        [DataMember(Name = "time")]
        public string Time { get; set; }

        [DataMember(Name = "state")]
        public bool State { get; set; }
    }

    [DataContract]
    public class TyrePressure
    {
        [DataMember(Name = "frontLeftTyrePressure")]
        public string FrontLeftTyrePressure { get; set; }

        [DataMember(Name = "frontRightTyrePressure")]
        public string FrontRightTyrePressure { get; set; }

        [DataMember(Name = "rearLeftTyrePressure")]
        public string RearLeftTyrePressure { get; set; }

        [DataMember(Name = "rearRightTyrePressure")]
        public string RearRightTyrePressure { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }
    }
}
