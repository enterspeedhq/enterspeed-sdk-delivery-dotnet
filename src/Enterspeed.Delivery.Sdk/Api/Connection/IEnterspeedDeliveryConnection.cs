using System.Net.Http;

namespace Enterspeed.Delivery.Sdk.Api.Connection
{
    public interface IEnterspeedDeliveryConnection
    {
        /// <summary>
        /// Gets the configured HttpClient.
        /// </summary>
        HttpClient HttpClientConnection { get; }

        /// <summary>
        /// Flushes/resets the current HttpClient.
        /// </summary>
        void Flush();
    }
}