using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace VolvoService.VolvoTypes.Position
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "position")]
        public Position Position { get; set; }
    }

    [DataContract]
    public class Position
    {
        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
