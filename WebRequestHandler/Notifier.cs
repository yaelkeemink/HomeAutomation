using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;
using WebRequestHandler.Entities;

namespace WebRequestHandler
{
    public interface INotifier
    {
        Task PostMessageAsync(int CurrentTravelTime, string state);
    }
    public class Notifier: INotifier
    {
        private readonly ILogger _logger;
        private readonly IHttpClientHandler _httpClient;
        private readonly Uri _uri;

        public Notifier(ILogger logger, IHttpClientHandler httpClient, string slackConnectionString)
        {
            _logger = logger;
            _httpClient = httpClient;
            _uri = new Uri(slackConnectionString);
        }

        public async Task PostMessageAsync(int CurrentTravelTime, string state)
        {
            var message = $"Current traveltime: {CurrentTravelTime} minutes, {state}";
            SlackMessage payload = new SlackMessage
            {
                Text = message
            };

            await PostMessageAsync(payload);
        }

        public async Task PostMessageAsync(SlackMessage payload)
        {
            try
            {
                var response = await _httpClient.PostJsonAsync(_uri, JsonConvert.SerializeObject(payload));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.Warning("Message posted to Slack, but result wasn't OK, instead it was; {0}", response);
                }
                else
                {
                    _logger.Information("Message succesfully posted to Slack");
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Something went wrong while posting slack message {payload.Text}\n{ e.Message}");
            }
        }
    }
}
