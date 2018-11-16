using AutomationCore;
using AutomationCore.EventBus;
using EasyNetQ.Topology;
using Newtonsoft.Json;
using NotificationService.Entities;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NotificationService
{
    public class Notifier : INotifier
    {
        private readonly ILogger _logger;
        private readonly IHttpClientHandler _httpClient;
        private readonly Uri _uri;
        private readonly IEventBusClient _rabbitMQClient;
        private readonly string _exchangeName = "HomeAutomationFanout";
        private readonly string _queuename = "NotificationService";

        public Notifier(ILogger logger, HttpClientHandler httpClient, string slackConnectionString)
        {
            _logger = logger;
            _httpClient = httpClient;
            _uri = new Uri(slackConnectionString);
            _rabbitMQClient = new RabbitMQClient()
                .DeclareExchange(_exchangeName, ExchangeType.Fanout)
                .DeclareQueue(_queuename)
                .BindQueue(_queuename, _exchangeName);
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

        private async Task PostMessageAsync(SlackMessage payload)
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

    public interface INotifier
    {
        Task PostMessageAsync(int CurrentTravelTime, string state);
    }
}
