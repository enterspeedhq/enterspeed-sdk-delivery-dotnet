using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Api.Services;
using Enterspeed.Delivery.Sdk.Domain.Connection;
using Enterspeed.Delivery.Sdk.Domain.Models;

namespace Enterspeed.Delivery.Sdk.Domain.Services
{
    public class EnterspeedDeliveryService : BaseEnterspeedDeliveryService, IEnterspeedDeliveryService
    {
        private readonly IJsonSerializer _serializer;

        public EnterspeedDeliveryService(
            EnterspeedDeliveryConnection enterspeedDeliveryConnection,
            IEnterspeedConfigurationProvider configurationProvider,
            IJsonSerializer jsonSerializer)
            : base(enterspeedDeliveryConnection, configurationProvider)
        {
            _serializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<DeliveryApiResponse> Fetch(string apiKey, CancellationToken cancellationToken, Action<DeliveryQueryBuilder> builder = null)
        {
            Validate(apiKey);
            var requestUri = RequestUri(builder);
            return await DeliveryApiResponse(apiKey, requestUri, cancellationToken);
        }

        public async Task<DeliveryApiResponse> Fetch(string apiKey, Action<DeliveryQueryBuilder> builder = null)
        {
            Validate(apiKey);
            var requestUri = RequestUri(builder);
            return await DeliveryApiResponse(apiKey, requestUri);
        }

        public async Task<DeliveryApiResponse<IContent>> FetchTyped(string apiKey, CancellationToken cancellationToken, Action<DeliveryQueryBuilder> builder = null)
        {
            Validate(apiKey);
            var requestUri = RequestUri(builder);
            return await DeliveryApiResponseTyped(apiKey, requestUri, cancellationToken);
        }

        public async Task<DeliveryApiResponse<IContent>> FetchTyped(string apiKey, Action<DeliveryQueryBuilder> builder = null)
        {
            Validate(apiKey);
            var requestUri = RequestUri(builder);
            return await DeliveryApiResponseTyped(apiKey, requestUri);
        }

        public async Task<DeliveryApiResponse> FetchMany(string apiKey, GetByIdsOrHandle getByIdsOrHandle, CancellationToken cancellationToken)
        {
            Validate(apiKey);

            var requestUri = RequestUri();

            var httpContent = new StringContent(_serializer.Serialize(getByIdsOrHandle), Encoding.UTF8, "application/json");
            return await DeliveryApiResponse(apiKey, requestUri, httpContent, cancellationToken);
        }

        public async Task<DeliveryApiResponse> FetchMany(string apiKey, GetByIdsOrHandle getByIdsOrHandle)
        {
            Validate(apiKey);

            var requestUri = RequestUri();

            var httpContent = new StringContent(_serializer.Serialize(getByIdsOrHandle), Encoding.UTF8, "application/json");
            return await DeliveryApiResponse(apiKey, requestUri, httpContent);
        }

        public async Task<DeliveryApiResponse<IContent>> FetchManyTyped(string apiKey, GetByIdsOrHandle getByIdsOrHandle, CancellationToken cancellationToken)
        {
            Validate(apiKey);

            var requestUri = RequestUri();

            var httpContent = new StringContent(_serializer.Serialize(getByIdsOrHandle), Encoding.UTF8, "application/json");
            return await DeliveryApiResponseTyped(apiKey, requestUri, httpContent, cancellationToken);
        }

        public async Task<DeliveryApiResponse<IContent>> FetchManyTyped(string apiKey, GetByIdsOrHandle getByIdsOrHandle)
        {
            Validate(apiKey);

            var requestUri = RequestUri();

            var httpContent = new StringContent(_serializer.Serialize(getByIdsOrHandle), Encoding.UTF8, "application/json");
            return await DeliveryApiResponseTyped(apiKey, requestUri, httpContent);
        }

        private async Task<DeliveryApiResponse<IContent>> DeliveryApiResponseTyped(string apiKey, Uri requestUri, CancellationToken? cancellationToken = null)
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

                return new DeliveryApiResponse<IContent>
                {
                    StatusCode = response.StatusCode,
                    Message = response.StatusCode != HttpStatusCode.OK && !string.IsNullOrWhiteSpace(responseString)
                        ? _serializer.Deserialize<DeliveryApiError>(responseString)?.Message
                        : null,
                    Response = response.StatusCode == HttpStatusCode.OK
                        ? _serializer.Deserialize<DeliveryResponse<IContent>>(responseString)
                        : null,
                    Headers = response.Headers
                };
            }
        }

        private async Task<DeliveryApiResponse<IContent>> DeliveryApiResponseTyped(string apiKey, Uri requestUri, HttpContent content, CancellationToken? cancellationToken = null)
        {
            HttpResponseMessage response;

            content.Headers.Add("X-Api-Key", apiKey);

            if (cancellationToken.HasValue)
            {
                response = await _enterspeedDeliveryConnection.HttpClientConnection.PostAsync(requestUri, content, cancellationToken.Value);
            }
            else
            {
                response = await _enterspeedDeliveryConnection.HttpClientConnection.PostAsync(requestUri, content);
            }

            var responseString = await response.Content.ReadAsStringAsync();

            return new DeliveryApiResponse<IContent>
            {
                StatusCode = response.StatusCode,
                Message = response.StatusCode != HttpStatusCode.OK && !string.IsNullOrWhiteSpace(responseString)
                    ? _serializer.Deserialize<DeliveryApiError>(responseString)?.Message
                    : null,
                Response = response.StatusCode == HttpStatusCode.OK
                    ? _serializer.Deserialize<DeliveryResponse<IContent>>(responseString)
                    : null,
                Headers = response.Headers
            };
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
                        : null,
                    Headers = response.Headers
                };
            }
        }

        private async Task<DeliveryApiResponse> DeliveryApiResponse(string apiKey, Uri requestUri, HttpContent content, CancellationToken? cancellationToken = null)
        {
            HttpResponseMessage response;

            content.Headers.Add("X-Api-Key", apiKey);

            if (cancellationToken.HasValue)
            {
                response = await _enterspeedDeliveryConnection.HttpClientConnection.PostAsync(requestUri, content, cancellationToken.Value);
            }
            else
            {
                response = await _enterspeedDeliveryConnection.HttpClientConnection.PostAsync(requestUri, content);
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
                    : null,
                Headers = response.Headers
            };
        }
    }
}