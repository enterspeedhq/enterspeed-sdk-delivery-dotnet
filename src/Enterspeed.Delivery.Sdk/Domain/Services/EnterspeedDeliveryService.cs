using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Api.Services;
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

        public async Task<DeliveryApiResponse> Fetch(string apiKey, Action<DeliveryQueryBuilder> builder = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey), "API key must be set");
            }

            var queryBuilder = new DeliveryQueryBuilder();
            builder?.Invoke(queryBuilder);

            var query = queryBuilder.Build();

            var requestUri = query.GetUri(_enterspeedDeliveryConnection.HttpClientConnection.BaseAddress, $"/v{_configurationProvider.Configuration.DeliveryVersion}");

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                requestMessage.Headers.Add("X-Api-Key", apiKey);

                var response =
                    await _enterspeedDeliveryConnection.HttpClientConnection.SendAsync(requestMessage);

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