using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Delivery.Sdk.Domain.SystemTextJson
{
    internal class DeliveryResponseConverter : JsonConverter<DeliveryResponse>
    {
        private static bool IsNullOrUnknown(JsonElement value)
        {
            return value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined;
        }

        public override DeliveryResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var output = new DeliveryResponse();

            var document = JsonDocument.ParseValue(ref reader);

            var metaElement = document.RootElement.GetProperty("meta");
            output.Meta = JsonSerializer.Deserialize<Meta>(metaElement.GetRawText(), options);

            var hasRoute = document.RootElement.TryGetProperty("route", out var route);
            var hasViews = document.RootElement.TryGetProperty("views", out var views);

            if (hasRoute)
            {
                output.Route = Get(route, options);
            }

            if (hasViews)
            {
                output.Views = Get(views, options);
            }

            return output;
        }

        private Dictionary<string, object> Get(JsonElement value, JsonSerializerOptions options)
        {
            if (IsNullOrUnknown(value))
            {
                return new Dictionary<string, object>();
            }

            var output = new Dictionary<string, object>();

            var elements = value.EnumerateObject();
            foreach (var element in elements)
            {
                if (IsNullOrUnknown(element.Value))
                {
                    output.Add(element.Name, null);
                }

                switch (element.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        output.Add(element.Name, element.Value.ToString());
                        break;
                    case JsonValueKind.Number:
                        output.Add(element.Name, element.Value.GetDecimal());
                        break;
                    case JsonValueKind.False:
                    case JsonValueKind.True:
                        output.Add(element.Name, element.Value.GetBoolean());
                        break;
                    case JsonValueKind.Array:
                        output.Add(element.Name, element.Value.EnumerateArray().Select(x => Get(x, options)));
                        break;
                    case JsonValueKind.Object:
                        output.Add(element.Name, Get(element.Value, options));
                        break;
                }
            }

            return output;
        }

        public override void Write(Utf8JsonWriter writer, DeliveryResponse value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Enterspeed Delivery SDK should not write any JSON.");
        }
    }
}