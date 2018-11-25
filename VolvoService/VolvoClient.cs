using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VolvoService.VolvoTypes.Trips;

namespace VolvoService
{
    public class VolvoClient : IVolvoClient
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
        private readonly string _vin;
        private readonly HttpClient _client;

        public VolvoClient(ILogger logger, IConfiguration config)
        {
            _vin = config["vin"];
            _baseUri = new Uri($"https://vocapi.wirelesscar.net/customerapi/rest/");   
            
            _client = new HttpClient();
            SetupClientHeaders(config["username"], config["password"]);           
        }

        public async Task StartHeatherAsync()
        {
            var uri = new Uri(_baseUri, $"{GetUriPrefix(true)}heater/start");            
            var response = await _client.PostAsync(uri, new StringContent("{}", Encoding.UTF8, "application/json"));
        }

        public async Task<VolvoTypes.Position.Position> RetrieveCurrentPositionAsync()
        {
            var response = await GetAsync<VolvoTypes.Position.Response>($"{GetUriPrefix()}position", _acceptHeaderValues[5]);
            return response?.Position;
        }

        public async Task<VolvoTypes.Status.Response> RetrieveStatusAsync()
        {
            var response = await GetAsync<VolvoTypes.Status.Response>($"{GetUriPrefix(true)}status", "");
            return response;
        }

        public async Task<IEnumerable<Trip>> RetrieveTripsAsync()
        {
            var response = await GetAsync<VolvoTypes.Trips.Response>($"{GetUriPrefix()}trips", _acceptHeaderValues[0]);
            return response?.Trips;
        }

        private async Task<T> GetAsync<T>(string relativeUri, string acceptHeaderValue)
        {
            var responseStream = await GetStreamAsync(relativeUri, acceptHeaderValue);

            using (var jsonTextReader = new JsonTextReader(new StreamReader(responseStream)))
            {
                return JsonSerializer.Create().Deserialize<T>(jsonTextReader);
            }
        }

        private Task<Stream> GetStreamAsync(string relativeUri, string acceptHeaderValue)
        {
            lock (_client)
            {
                _client.DefaultRequestHeaders.Accept.Clear();
                if (!string.IsNullOrEmpty(acceptHeaderValue))
                {
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeaderValue));
                }
                return _client.GetStreamAsync(new Uri(_baseUri, relativeUri));
            }
        }
        
        private string GetUriPrefix(bool v3 = false)
        {
            if (v3)
            {
                return $"v3.0/vehicles/{_vin}/";
            }
            return $"vehicles/{_vin}/";
        }

        private void SetupClientHeaders(string username, string password)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password))));
            _client.DefaultRequestHeaders.Add("cache-control", "no-cache");
            _client.DefaultRequestHeaders.Add("x-device-id", "Device");
            _client.DefaultRequestHeaders.Add("X-Originator-Type", "app");
            _client.DefaultRequestHeaders.Add("X-OS-Type", "Android");
            _client.DefaultRequestHeaders.Add("x-os-version", "22");
        }
    }

    public interface IVolvoClient
    {
        Task<VolvoTypes.Position.Position> RetrieveCurrentPositionAsync();
        Task<VolvoTypes.Status.Response> RetrieveStatusAsync();
        Task<IEnumerable<Trip>> RetrieveTripsAsync();
        Task StartHeatherAsync();
    }
}
