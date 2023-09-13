using System;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;

namespace Enterspeed.Delivery.Sdk.Api.Services
{
    public interface IEnterspeedDeliveryService
    {
        Task<DeliveryApiResponse<IContent>> FetchTyped(string apiKey, Action<DeliveryQueryBuilder> builder = null);
        Task<DeliveryApiResponse<IContent>> FetchTyped(string apiKey, CancellationToken cancellationToken, Action<DeliveryQueryBuilder> builder = null);
        Task<DeliveryApiResponse> Fetch(string apiKey, Action<DeliveryQueryBuilder> builder = null);
        Task<DeliveryApiResponse> Fetch(string apiKey, CancellationToken cancellationToken, Action<DeliveryQueryBuilder> builder = null);
    }
}