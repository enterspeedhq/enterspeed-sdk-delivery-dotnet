using System;
using Enterspeed.Delivery.Sdk.Configuration;

namespace Enterspeed.Delivery.Sdk.Api.Providers
{
    public class EnterspeedConfigurationProvider : IEnterspeedConfigurationProvider
    {
        public EnterspeedDeliveryConfiguration Configuration { get; set; }

        public EnterspeedConfigurationProvider(EnterspeedDeliveryConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
    }
}