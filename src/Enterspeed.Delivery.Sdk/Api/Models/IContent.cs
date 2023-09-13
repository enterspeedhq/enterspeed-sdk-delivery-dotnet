using System.Text.Json;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public interface IContent
    {
        JsonElement Value { get; }

        T GetContent<T>();
        T GetContent<T>(string alias);
    }
}