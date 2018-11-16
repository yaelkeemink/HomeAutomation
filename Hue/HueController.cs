using AutomationCore.EventBus;
using EasyNetQ.Topology;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.Interfaces;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HueService
{
    public interface IHueController
    {
        void SetLampColorAsync(string lampId, string colorCode);
    }
    public class HueController : IHueController
    {
        private readonly ILogger _logger;
        private readonly ILocalHueClient _hueClient;
        private readonly IEventBusClient _rabbitMQClient;
        private readonly string _exchangeName = "HomeAutomationFanout";
        private readonly string _queuename = "HueService";

        public HueController(ILogger logger, string clientIP, string username)
        {
            _logger = logger;
            _hueClient = new LocalHueClient(clientIP);
            _hueClient.Initialize(username);
            _rabbitMQClient = new RabbitMQClient()
                .DeclareExchange(_exchangeName, ExchangeType.Fanout)
                .DeclareQueue(_queuename)
                .BindQueue(_queuename, _exchangeName);
        }
        public async void SetLampColorAsync(string lightId, string colorCode)
        {
            RGBColor rgbColor = new RGBColor(colorCode);
            var command = new LightCommand();
            command.TurnOn().SetColor(rgbColor);

            var light = await GetLight(lightId);
            if (light != null)
            {
                _logger.Information($"Changing light status for lamp {lightId} to {rgbColor}");
                await _hueClient.SendCommandAsync(command, new List<string> { light.Id });
            }
        }

        public async Task<Light> GetLight(string lightId)
        {
            IEnumerable<Light> lights = await _hueClient.GetLightsAsync();
            return lights?.FirstOrDefault(l => l.UniqueId == lightId);
        }
    }
}
