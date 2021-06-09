namespace Enterspeed.Delivery.Sdk.Configuration
{
    public class EnterspeedDeliveryConfiguration
    {
        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        public string BaseUrl { get; set; } = "https://delivery.enterspeed.com";

        /// <summary>
        /// Gets or sets timeout in seconds. Default: 60 seconds.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 60;

        /// <summary>
        /// Gets the current version for the Delivery Endpoint.
        /// </summary>
        public string DeliveryVersion => "1";
    }
}