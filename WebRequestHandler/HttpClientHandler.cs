using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebRequestHandler
{
    public interface IHttpClientHandler
    {
        Task<HttpResponseMessage> GetAsync(Uri uri, Action<HttpRequestMessage> preAction = null);
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, Action<HttpRequestMessage> preAction = null);
        Task<HttpResponseMessage> PostJsonAsync(Uri uri, string content, Action<HttpRequestMessage> preAction = null);
    }

    public class HttpClientHandler : IHttpClientHandler
    {
        private HttpClient _httpClient;

        public HttpClientHandler()
        {
            _httpClient = new HttpClient();
        }

        public Task<HttpResponseMessage> GetAsync(Uri uri, Action<HttpRequestMessage> preAction = null)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            preAction?.Invoke(httpRequestMessage);

            return _httpClient.SendAsync(httpRequestMessage);
        }

        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, Action<HttpRequestMessage> preAction = null)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };

            preAction?.Invoke(httpRequestMessage);

            return _httpClient.SendAsync(httpRequestMessage);
        }

        public Task<HttpResponseMessage> PostJsonAsync(Uri uri, string content, Action<HttpRequestMessage> preAction = null)
        {
            return PostAsync(uri, new StringContent(content, Encoding.UTF8, "application/json"), preAction);
        }
    }
}
