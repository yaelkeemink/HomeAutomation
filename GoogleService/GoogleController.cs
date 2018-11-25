using AutomationCore.EventBus.Messages.Events;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebRequestHandler;

namespace GoogleService
{
    public class GoogleController : IGoogleController
    {      
        private readonly ILogger _logger;
        private readonly IHttpClientHandler _httpHandler;
        private readonly IConfiguration _config;

        public GoogleController(ILogger logger, IHttpClientHandler httpHandler, IConfiguration config)
        {
            _logger = logger;
            _httpHandler = httpHandler;
            _config = config;
        }
        
        
        public async Task<JObject> GetCurrentWeather(double lat, double lon)
        {
            var weatherApiKey = _config["WeatherApiKey"];
            Uri uri = new Uri($"{_config["WeatherBaseUri"]}lat={lat}&lon={lon}&appid={_config["WeatherApiKey"]}");
            var response = await _httpHandler.GetAsync(uri);
            return await GetJsonFromHttpResponse(response);
        }
        public async Task<JObject> GetCurrentTravelTime(double lat, double lon)
        {
            string to = _config["HomeAdress"];
            if(DateTime.Now.TimeOfDay < new TimeSpan(12,0,0))
            {
                to = _config["WorkAdress"];
            }
            Uri uri = new Uri($"{_config["TrafficBaseUri"]}origin={lat},{lon}&destination={to}&departure_time=now&key={_config["GoogleApiKey"]}");
            var response = await _httpHandler.GetAsync(uri);
            return await GetJsonFromHttpResponse(response);            
        }

        private async Task<JObject> GetJsonFromHttpResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(content);
            _logger.Information(jsonObject.ToString());
            return jsonObject;
        }
    }
    public interface IGoogleController
    {
        Task<JObject> GetCurrentWeather(double lat, double lon);
        Task<JObject> GetCurrentTravelTime(double lat, double lon);
    }
}
