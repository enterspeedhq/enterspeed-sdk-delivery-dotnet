using System;
using System.Text.Json;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public class Content : IContent
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public Content(JsonElement value, JsonSerializerOptions serializerOptions)
        {
            _serializerOptions = serializerOptions;
            Value = value;
        }

        public JsonElement Value { get; set; }
        public T GetContent<T>()
        {
            #if NET6_0_OR_GREATER
            return Value.Deserialize<T>(_serializerOptions);
            #endif
            throw new NotImplementedException();
        }

        public T GetContent<T>(string propertyName)
        {
            #if NET6_0_OR_GREATER
            var hasValue = Value.TryGetProperty(propertyName, out var value);
            return hasValue
                ? value.Deserialize<T>(_serializerOptions)
                : default;
            #endif
            throw new NotImplementedException();
        }
    }
}