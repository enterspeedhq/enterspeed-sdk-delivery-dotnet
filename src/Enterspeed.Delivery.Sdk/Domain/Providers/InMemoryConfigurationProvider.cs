using System;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Configuration;

namespace Enterspeed.Delivery.Sdk.Domain.Providers
{
    public class InMemoryConfigurationProvider : IEnterspeedConfigurationProvider
    {
        public InMemoryConfigurationProvider(EnterspeedDeliveryConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public EnterspeedDeliveryConfiguration Configuration { get; }
    }
}