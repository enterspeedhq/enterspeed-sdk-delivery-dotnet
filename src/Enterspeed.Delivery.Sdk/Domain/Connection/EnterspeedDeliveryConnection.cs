using System;
using System.Net.Http;
using Enterspeed.Delivery.Sdk.Api.Connection;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Configuration;
namespace Enterspeed.Delivery.Sdk.Domain.Connection
{
    public sealed class EnterspeedDeliveryConnection : IEnterspeedDeliveryConnection, IDisposable
    {
        private readonly int _connectionTimeout;
        private DateTime? _connectionEstablishedDate;
        private HttpClient _httpClientConnection;

        public EnterspeedDeliveryConnection(IEnterspeedConfigurationProvider configurationProvider)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException(nameof(configurationProvider));
            }

            BaseUrl = configurationProvider.Configuration?.BaseUrl;
            _connectionTimeout = configurationProvider.Configuration?.ConnectionTimeout ?? 60;
        }

        private string BaseUrl { get; }

        public void Dispose()
        {
            _httpClientConnection?.Dispose();
        }

        public HttpClient HttpClientConnection
        {
            get
            {
                if (_httpClientConnection == null
                    || !_connectionEstablishedDate.HasValue
                    || (DateTime.Now - _connectionEstablishedDate.Value).TotalSeconds > _connectionTimeout)
                {
                    Connect();
                }

                return _httpClientConnection;
            }
        }

        public void Flush()
        {
            _httpClientConnection = null;
            _connectionEstablishedDate = null;
        }

        private void Connect()
        {
            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                throw new ConfigurationException(nameof(BaseUrl));
            }

            HttpClient httpClient;

#if NETCOREAPP2_1_OR_GREATER
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromSeconds(60)
            };

            httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
#else
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
#endif

            _httpClientConnection = httpClient;

            _httpClientConnection.DefaultRequestHeaders.Add("Accept", "application/json");

            _connectionEstablishedDate = DateTime.Now;
        }
    }
}
