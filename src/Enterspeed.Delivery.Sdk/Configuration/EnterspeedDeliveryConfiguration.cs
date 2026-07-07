namespace Enterspeed.Delivery.Sdk.Configuration
{
    public class EnterspeedDeliveryConfiguration
    {
        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        public string BaseUrl { get; set; } = "https://delivery.enterspeed.com";

        /// <summary>
        /// Gets or sets the connection refresh interval in seconds. Default: 60.
        /// This is not a request timeout: requests use <c>HttpClient.Timeout</c>, which this SDK leaves
        /// at its default of 100 seconds.
        /// On .NET Core 2.1+/.NET 5+ this value configures <c>SocketsHttpHandler.PooledConnectionLifetime</c>
        /// — pooled connections are re-established at this interval so DNS changes are picked up — and
        /// the <c>HttpClient</c> instance itself is only replaced by
        /// <c>IEnterspeedDeliveryConnection.Flush()</c>.
        /// On .NET Framework (the netstandard2.0 asset) it is the interval at which the internal
        /// <c>HttpClient</c> is recreated, which is the DNS-rotation mechanism available there.
        /// Values less than or equal to 0 are treated as 1 second.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 60;

        /// <summary>
        /// Gets or sets the current version for the Delivery Endpoint.
        /// </summary>
        public string DeliveryVersion { get; set; } = "2";
    }
}