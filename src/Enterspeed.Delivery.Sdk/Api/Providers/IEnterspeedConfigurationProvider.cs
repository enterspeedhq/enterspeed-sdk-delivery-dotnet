using Enterspeed.Delivery.Sdk.Configuration;

namespace Enterspeed.Delivery.Sdk.Api.Providers
{
    public interface IEnterspeedConfigurationProvider
    {
        EnterspeedDeliveryConfiguration Configuration { get; }
    }
}