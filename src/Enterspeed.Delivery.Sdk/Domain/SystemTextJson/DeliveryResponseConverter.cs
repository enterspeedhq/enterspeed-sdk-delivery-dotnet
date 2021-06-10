using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Delivery.Sdk.Domain.SystemTextJson
{
    internal class DeliveryResponseConverter : JsonConverter<DeliveryResponse>
    {
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
                output.Route = Get(route.EnumerateObject(), options);
            }

            if (hasViews)
            {
                output.Views = Get(views.EnumerateObject(), options);
            }

            return output;
        }

        private Dictionary<string, object> Get(
            JsonElement.ObjectEnumerator objectEnumerator,
            JsonSerializerOptions options)
        {
            var output = new Dictionary<string, object>();

            foreach (var element in objectEnumerator)
            {
                output.Add(element.Name, GetElementValue(element.Value, options));
            }

            return output;
        }

        private IEnumerable<object> Get(
            JsonElement.ArrayEnumerator arrayEnumerator,
            JsonSerializerOptions options)
        {
            var output = new List<object>();

            foreach (var element in arrayEnumerator)
            {
                output.Add(GetElementValue(element, options));
            }

            return output;
        }

        private object GetElementValue(JsonElement element, JsonSerializerOptions options)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.ToString();
                case JsonValueKind.Number:
                    return element.GetDecimal();
                case JsonValueKind.False:
                case JsonValueKind.True:
                    return element.GetBoolean();
                case JsonValueKind.Array:
                    return Get(element.EnumerateArray(), options);
                case JsonValueKind.Object:
                    return Get(element.EnumerateObject(), options);
                default:
                    return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, DeliveryResponse value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Enterspeed Delivery SDK should not write any JSON.");
        }
    }
}