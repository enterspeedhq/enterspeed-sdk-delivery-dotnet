using System;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Delivery.Sdk.Api.Services
{
    public interface IEnterspeedDeliveryService
    {
        Task<DeliveryApiResponse> Fetch(string apiKey, Action<DeliveryQueryBuilder> builder = null);
    }
}