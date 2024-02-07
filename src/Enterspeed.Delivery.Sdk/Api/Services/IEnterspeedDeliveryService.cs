using System;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Domain.Models;

namespace Enterspeed.Delivery.Sdk.Api.Services
{
    public interface IEnterspeedDeliveryService
    {
        Task<DeliveryApiResponse> Fetch(string apiKey, Action<DeliveryQueryBuilder> builder = null);
        Task<DeliveryApiResponse> Fetch(string apiKey, CancellationToken cancellationToken, Action<DeliveryQueryBuilder> builder = null);
        Task<DeliveryApiResponse> FetchMultiple(string apiKey, GetByIdsOrHandle body, CancellationToken cancellationToken);
        Task<DeliveryApiResponse> FetchMultiple(string apiKey, GetByIdsOrHandle body);
    }
}