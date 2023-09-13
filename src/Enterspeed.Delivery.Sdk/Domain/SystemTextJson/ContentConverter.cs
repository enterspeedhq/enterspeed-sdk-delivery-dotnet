using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Delivery.Sdk.Domain.SystemTextJson
{
    public class ContentConverter : JsonConverter<IContent>
    {
        public override IContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Content(JsonSerializer.Deserialize<JsonElement>(ref reader, options), options);
        }

        public override void Write(Utf8JsonWriter writer, IContent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}