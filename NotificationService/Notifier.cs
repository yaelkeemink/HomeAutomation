using AutomationCore;
using Newtonsoft.Json;
using NotificationService.Entities;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;
using WebRequestHandler;

namespace NotificationService
{
    public class Notifier : INotifier
    {
        private readonly Uri _uri;
        private readonly ILogger _logger;
        private readonly IHttpClientHandler _httpClient;        

        public Notifier(ILogger logger, IHttpClientHandler httpClient, string slackConnectionString)
        {
            _logger = logger;
            _httpClient = httpClient;
            _uri = new Uri(slackConnectionString);
        }

        

        public async Task PostMessageAsync(string message)
        {
            
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

    public interface INotifier
    {
        Task PostMessageAsync(string message);
        Task PostMessageAsync(SlackMessage payload);
    }
}
