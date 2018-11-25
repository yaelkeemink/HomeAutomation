using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VolvoService.VolvoTypes.Trips
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "trips")]
        public IEnumerable<Trip> Trips { get; set; }
    }

    [DataContract]
    public class Trip
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "userNotes")]
        public string UserNotes { get; set; }

        [DataMember(Name = "tripDetails")]
        public IEnumerable<TripDetails> Details { get; set; }
    }

    [DataContract]
    public class TripDetails
    {
        [DataMember(Name = "distance")]
        public int Distance { get; set; }

        [DataMember(Name = "startTime")]
        public DateTime StartTime { get; set; }

        [DataMember(Name = "endTime")]
        public DateTime EndTime { get; set; }

        [DataMember(Name = "startPosition")]
        public Position StartPosition { get; set; }

        [DataMember(Name = "endPosition")]
        public Position EndPosition { get; set; }
    }

    [DataContract]
    public class Position
    {
        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "streetAddress")]
        public string StreetAddress { get; set; }

        [DataMember(Name = "postalCode")]
        public string PostalCode { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "ISO2CountryCode")]
        public string ISO2CountryCode { get; set; }

        [DataMember(Name = "Region")]
        public string Region { get; set; }
    }
}
