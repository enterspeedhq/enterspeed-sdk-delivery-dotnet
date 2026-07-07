using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Configuration;
using Enterspeed.Delivery.Sdk.Domain.Connection;

namespace Enterspeed.Delivery.Sdk.Domain.Services
{
    public abstract class BaseEnterspeedDeliveryService
    {
        protected readonly EnterspeedDeliveryConnection _enterspeedDeliveryConnection;
        private readonly IEnterspeedConfigurationProvider _configurationProvider;

        protected BaseEnterspeedDeliveryService(
            EnterspeedDeliveryConnection enterspeedDeliveryConnection,
            IEnterspeedConfigurationProvider configurationProvider)
        {
            _enterspeedDeliveryConnection = enterspeedDeliveryConnection ?? throw new ArgumentNullException(nameof(enterspeedDeliveryConnection));
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        protected Uri RequestUri(Action<DeliveryQueryBuilder> builder = null)
        {
            return RequestUri(_enterspeedDeliveryConnection.HttpClientConnection, builder);
        }

        /// <summary>
        /// Builds the request URI from the given client's base address. Capture
        /// <see cref="EnterspeedDeliveryConnection.HttpClientConnection"/> once per operation and use the
        /// same client to build the URI and send the request: the connection can swap clients between
        /// getter calls (<see cref="EnterspeedDeliveryConnection.Flush"/>, or the timed refresh on the
        /// netstandard2.0 asset), and mixing two generations within one operation is unsafe if their
        /// configuration ever differs.
        /// </summary>
        protected Uri RequestUri(HttpClient client, Action<DeliveryQueryBuilder> builder = null)
        {
            var queryBuilder = new DeliveryQueryBuilder();
            builder?.Invoke(queryBuilder);

            var query = queryBuilder.Build();

            Uri requestUri;

            if (!query.IsDeliveryApiUrl)
            {
                requestUri = query.GetUri(client.BaseAddress,
                    $"/v{_configurationProvider.Configuration.DeliveryVersion}");
            }
            else
            {
                requestUri = query.GetUri();
            }

            return requestUri;
        }
    
        protected void Validate(string apiKey)
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

        protected async Task<HttpResponseMessage> SendAsync(CancellationToken? cancellationToken, HttpRequestMessage requestMessage)
        {
            var client = _enterspeedDeliveryConnection.HttpClientConnection;

            HttpResponseMessage response;
            if (cancellationToken.HasValue)
            {
                response = await client.SendAsync(requestMessage, cancellationToken.Value);
            }
            else
            {
                response = await client.SendAsync(requestMessage);
            }

            return response;
        }
    }
}