using Enterspeed.Delivery.Sdk.Api.Connection;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Api.Services;
using Enterspeed.Delivery.Sdk.Configuration;
using Enterspeed.Delivery.Sdk.Domain.Connection;
using Enterspeed.Delivery.Sdk.Domain.Services;
using Enterspeed.Delivery.Sdk.Domain.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace Enterspeed.Delivery.Sdk.Api.Extensions
{
    public static class EnterspeedServiceCollectionExtension
    {
        public static IServiceCollection AddEnterspeedDeliveryService(this IServiceCollection services, EnterspeedDeliveryConfiguration enterspeedDeliveryConfiguration = null)
        {
            services.AddTransient<IEnterspeedDeliveryService, EnterspeedDeliveryService>();
            services.AddTransient<IJsonSerializer, SystemTextJsonSerializer>();

            // The connection is a thread-safe, IDisposable holder of the shared HttpClient and must be a
            // singleton: a transient IDisposable is captured by the container for disposal on every
            // resolution, and creating an HttpClient per resolution is the documented socket-exhaustion
            // pattern. Configuration is captured once, when the singleton is first created.
            services.AddSingleton<EnterspeedDeliveryConnection>();
            services.AddSingleton<IEnterspeedDeliveryConnection>(sp => sp.GetRequiredService<EnterspeedDeliveryConnection>());
            services.AddSingleton<IEnterspeedConfigurationProvider>(new EnterspeedConfigurationProvider(enterspeedDeliveryConfiguration ?? new EnterspeedDeliveryConfiguration()));
            return services;
        }
    }
}
