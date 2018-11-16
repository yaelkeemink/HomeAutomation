using AutomationCore.EventBus;
using EasyNetQ.Topology;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Threading.Tasks;
using WebRequestHandler;

namespace GoogleService
{
    public interface IGoogleController
    {
        Task<int> GetCurrentTravelTime(string from, string to);
    }
    public class GoogleController : IGoogleController
    {
        private readonly string _baseUri;
        private readonly IEventBusClient _rabbitMQClient;
        private readonly string _apiKey;
        private readonly ILogger _logger;
        private readonly IHttpClientHandler _httpHandler;
        private readonly string _exchangeName = "HomeAutomationFanout";
        private readonly string _queuename = "GoogleService";

        public GoogleController(ILogger logger, IHttpClientHandler httpHandler, string baseUri, string apiKey)
        {
            _logger = logger;
            _httpHandler = httpHandler;
            _apiKey = apiKey;
            _baseUri = baseUri;
            _rabbitMQClient = new RabbitMQClient()
                .DeclareExchange(_exchangeName, ExchangeType.Fanout)
                .DeclareQueue(_queuename)
                .BindQueue(_queuename, _exchangeName);
        }
        public async Task<int> GetCurrentTravelTime(string from, string to)
        {
            Uri uri = new Uri($"{_baseUri}origin={from}&destination={to}&departure_time=now&key={_apiKey}");
            var response = await _httpHandler.GetAsync(uri);
            var content = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(content);
            _logger.Information(jsonObject.ToString());
            return (int)jsonObject["routes"][0]["legs"][0]["duration_in_traffic"]["value"];
        }
    }
}
