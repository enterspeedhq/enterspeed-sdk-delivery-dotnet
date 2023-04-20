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
            services.AddTransient<EnterspeedDeliveryConnection>();
            services.AddSingleton<IEnterspeedConfigurationProvider>(new EnterspeedConfigurationProvider(enterspeedDeliveryConfiguration ?? new EnterspeedDeliveryConfiguration()));
            return services;
        }
    }
}
