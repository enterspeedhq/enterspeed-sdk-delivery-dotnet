using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Api.Services;
using Enterspeed.Delivery.Sdk.Configuration;
using Enterspeed.Delivery.Sdk.Domain.Connection;

namespace Enterspeed.Delivery.Sdk.Domain.Services
{
    public class EnterspeedDeliveryService : IEnterspeedDeliveryService
    {
        private readonly EnterspeedDeliveryConnection _enterspeedDeliveryConnection;
        private readonly IEnterspeedConfigurationProvider _configurationProvider;
        private readonly IJsonSerializer _serializer;

        public EnterspeedDeliveryService(
            EnterspeedDeliveryConnection enterspeedDeliveryConnection,
            IEnterspeedConfigurationProvider configurationProvider,
            IJsonSerializer jsonSerializer)
        {
            _enterspeedDeliveryConnection = enterspeedDeliveryConnection ?? throw new ArgumentNullException(nameof(enterspeedDeliveryConnection));
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            _serializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<DeliveryApiResponse> Fetch(string apiKey, CancellationToken cancellationToken, Action<DeliveryQueryBuilder> builder = null)
        {
            Validate(apiKey);
            
            var requestUri = RequestUri(apiKey, builder);
            return await DeliveryApiResponse(apiKey, requestUri, cancellationToken);
        }

        public async Task<DeliveryApiResponse> Fetch(string apiKey, Action<DeliveryQueryBuilder> builder = null)
        {
            Validate(apiKey);

            var requestUri = RequestUri(apiKey, builder);
            return await DeliveryApiResponse(apiKey, requestUri);
        }

        private Uri RequestUri(string apiKey, Action<DeliveryQueryBuilder> builder)
        {
            var queryBuilder = new DeliveryQueryBuilder();
            builder?.Invoke(queryBuilder);

            var query = queryBuilder.Build();

            Uri requestUri;

            if (!query.IsDeliveryApiUrl)
            {
                requestUri = query.GetUri(_enterspeedDeliveryConnection.HttpClientConnection.BaseAddress,
                    $"/v{_configurationProvider.Configuration.DeliveryVersion}");
            }
            else
            {
                requestUri = query.GetUri();
            }

            return requestUri;
        }

        private void Validate(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey), "API key must be set");
            }

            if (string.IsNullOrWhiteSpace(_configurationProvider.Configuration.DeliveryVersion))
            {
                throw new ConfigurationException(nameof(EnterspeedDeliveryConfiguration.DeliveryVersion));
            }
        }

        private async Task<DeliveryApiResponse> DeliveryApiResponse(string apiKey, Uri requestUri, CancellationToken? cancellationToken = null)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                HttpResponseMessage response;

                requestMessage.Headers.Add("X-Api-Key", apiKey);

                if (cancellationToken.HasValue)
                {
                    response = await _enterspeedDeliveryConnection.HttpClientConnection.SendAsync(requestMessage, cancellationToken.Value);
                }
                else
                {
                    response = await _enterspeedDeliveryConnection.HttpClientConnection.SendAsync(requestMessage);
                }

                var responseString = await response.Content.ReadAsStringAsync();

                return new DeliveryApiResponse
                {
                    StatusCode = response.StatusCode,
                    Message = response.StatusCode != HttpStatusCode.OK && !string.IsNullOrWhiteSpace(responseString)
                        ? _serializer.Deserialize<DeliveryApiError>(responseString)?.Message
                        : null,
                    Response = response.StatusCode == HttpStatusCode.OK
                        ? _serializer.Deserialize<DeliveryResponse>(responseString)
                        : null
                };
            }
        }
    }
}