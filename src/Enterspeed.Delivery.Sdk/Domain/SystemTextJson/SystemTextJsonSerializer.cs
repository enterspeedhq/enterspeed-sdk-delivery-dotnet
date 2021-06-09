#if NETSTANDARD2_0
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enterspeed.Delivery.Sdk.Api.Services;

namespace Enterspeed.Delivery.Sdk.Domain.SystemTextJson
{
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public SystemTextJsonSerializer(IList<JsonConverter> converters = null)
        {
            if (converters != null && converters.Any())
            {
                foreach (var converter in converters)
                {
                    _options.Converters.Add(converter);
                }
            }
            else
            {
                _options.Converters.Add(new DeliveryResponseConverter());
            }
        }

        public string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, _options);
        }

        public T Deserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value, _options);
        }
    }
}
#endif