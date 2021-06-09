using System;
using System.Net.Http;
using Enterspeed.Delivery.Sdk.Api.Connection;
using Enterspeed.Delivery.Sdk.Api.Providers;

namespace Enterspeed.Delivery.Sdk.Domain.Connection
{
    public class EnterspeedDeliveryConnection : IEnterspeedDeliveryConnection
    {
        private readonly IEnterspeedConfigurationProvider _configurationProvider;
        private HttpClient _httpClientConnection;
        private DateTime? _connectionEstablishedDate;
        private readonly int _connectionTimeout;

        public EnterspeedDeliveryConnection(IEnterspeedConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));

            BaseUrl = configurationProvider.Configuration?.BaseUrl;
            _connectionTimeout = configurationProvider.Configuration?.ConnectionTimeout ?? 60;
        }

        private string BaseUrl { get; }

        public HttpClient HttpClientConnection
        {
            get
            {
                if (_httpClientConnection == null
                    || !_connectionEstablishedDate.HasValue
                    || ((DateTime.Now - _connectionEstablishedDate.Value).TotalSeconds > _connectionTimeout))
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
                throw new Exception("BaseUrl is missing in the connection.");
            }

            _httpClientConnection = new HttpClient
            {
                BaseAddress = new System.Uri(BaseUrl)
            };

            _httpClientConnection.DefaultRequestHeaders.Add("Accept", "application/json");

            _connectionEstablishedDate = DateTime.Now;
        }
    }
}