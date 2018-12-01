using Microsoft.Extensions.Configuration;
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
    public class HueController : IHueController
    {
        private readonly ILogger _logger;
        private readonly IConfigurationSection _config;        
        private readonly ILocalHueClient _hueClient;

        public HueController(ILogger logger, IConfigurationSection config, ILocalHueClient hueClient)
        {
            _logger = logger;
            _config = config;
            _hueClient = hueClient;
            _hueClient.Initialize(config["UserName"]);
        }

        public string GetTrafficColor(int currentTravelTime, int defaultTravelTime)
        {
            //White
            var color = "FFFFFF";            
            if (currentTravelTime >= defaultTravelTime + 10)
            {
                //Red
                color = "FF0000";
            }
            else if (currentTravelTime >= defaultTravelTime + 5)
            {
                //Orange
                color = "FFA500";
            }
            return color;            
        }

        public async Task<Light> SetLampColorAsync(string colorCode, string lightId = "")
        {
            if(lightId == "")
            {
                lightId = _config["MainLight"];
            }

            RGBColor rgbColor = new RGBColor(colorCode);
            var command = new LightCommand();
            command.TurnOn().SetColor(rgbColor);

            var light = await GetLight(lightId);
            if (light != null)
            {
                _logger.Information($"Changing light color for lamp {lightId} to {colorCode}");
                await _hueClient.SendCommandAsync(command, new List<string> { light.Id });
            }
            else
            {
                _logger.Error($"Could not find light for light id:{lightId}");
            }
            return light;
        }

        public async Task<Light> GetLight(string lightId)
        {
            IEnumerable<Light> lights = await _hueClient.GetLightsAsync();
            return lights?.FirstOrDefault(l => l.UniqueId == lightId);
        }
    }

    public interface IHueController
    {
        Task<Light> GetLight(string lightId);
        Task<Light> SetLampColorAsync(string colorCode, string lampId = "");
        string GetTrafficColor(int currentTravelTime, int defaultTravelTime);
    }
}
