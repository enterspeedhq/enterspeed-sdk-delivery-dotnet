using System.Text.Json;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public interface IContent
    {
        JsonElement Value { get; }

        /// <summary>
        /// Returns the content of the view serialized as a custom type
        /// </summary>
        T GetContent<T>();

        /// <summary>
        /// Returns the content of first level property on the view serialized as a custom type
        /// </summary>
        T GetContent<T>(string propertyName);
    }
}