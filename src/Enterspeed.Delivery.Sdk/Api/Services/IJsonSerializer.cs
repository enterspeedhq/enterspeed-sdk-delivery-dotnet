namespace Enterspeed.Delivery.Sdk.Api.Services
{
    public interface IJsonSerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string value);
    }
}