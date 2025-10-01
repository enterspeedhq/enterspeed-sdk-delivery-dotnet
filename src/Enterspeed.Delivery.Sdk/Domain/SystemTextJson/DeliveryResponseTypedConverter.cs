using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Delivery.Sdk.Domain.SystemTextJson
{
    internal class DeliveryResponseTypedConverter : JsonConverter<DeliveryResponse<IContent>>
    {
        public override DeliveryResponse<IContent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var output = new DeliveryResponse<IContent>();

            var document = JsonDocument.ParseValue(ref reader);

            var metaElement = document.RootElement.GetProperty("meta");
            output.Meta = JsonSerializer.Deserialize<Meta>(metaElement.GetRawText(), options);

            var hasRoute = document.RootElement.TryGetProperty("route", out var route);
            var hasViews = document.RootElement.TryGetProperty("views", out var views);

            if (hasRoute && IsValueKindNotNull(route))
            {
                output.Route = GetElementValue(route, options);
            }

            if (hasViews && views.ValueKind == JsonValueKind.Object)
            {
                output.Views = Get(views.EnumerateObject(), options);
            }

            return output;
        }

        private static bool IsValueKindNotNull(JsonElement route)
        {
            return route.ValueKind != JsonValueKind.Null && route.ValueKind != JsonValueKind.Undefined;
        }

        private Dictionary<string, IContent> Get(
            JsonElement.ObjectEnumerator objectEnumerator,
            JsonSerializerOptions options)
        {
            var output = new Dictionary<string, IContent>();

            foreach (var element in objectEnumerator)
            {
                output.Add(element.Name, GetElementValue(element.Value, options));
            }

            return output;
        }

        private IEnumerable<IContent> Get(
            JsonElement.ArrayEnumerator arrayEnumerator,
            JsonSerializerOptions options)
        {
            var output = new List<IContent>();

            foreach (var element in arrayEnumerator)
            {
                output.Add(GetElementValue(element, options));
            }

            return output;
        }

        private IContent GetElementValue(JsonElement element, JsonSerializerOptions options)
        {
            return new Content(element, options);
        }

        public override void Write(Utf8JsonWriter writer, DeliveryResponse<IContent> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Enterspeed Delivery SDK should not write any JSON.");
        }
    }
}