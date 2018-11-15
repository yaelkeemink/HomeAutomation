using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WebRequestHandler.Volvo
{
    public class VolvoClient
    {
        private static readonly string[] _acceptHeaderValues = {
            "application/vnd.wirelesscar.com.voc.Trip.v4+json",
            "application/vnd.wirelesscar.com.voc.AppUser.v4+json",
            "application/vnd.wirelesscar.com.voc.VehicleStatus.v4+json",
            "application/vnd.wirelesscar.com.voc.Service.v4+json",
            "application/vnd.wirelesscar.com.voc.VehicleAttributes.v4+json",
            "application/vnd.wirelesscar.com.voc.Position.v4+json"
        };

        private readonly Uri _baseUri;
        private readonly HttpClient _client;

        public VolvoClient(string username, string password, string vin)
        {
            _baseUri = new Uri($"https://vocapi.wirelesscar.net/customerapi/rest/vehicles/{vin}/");

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password))));
            _client.DefaultRequestHeaders.Add("X-Originator-Type", "app");
            _client.DefaultRequestHeaders.Add("X-OS-Type", "Android");
        }
    }
}
