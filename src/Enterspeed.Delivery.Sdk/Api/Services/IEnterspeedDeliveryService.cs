using System;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Domain.Models;

namespace Enterspeed.Delivery.Sdk.Api.Services
{
    public interface IEnterspeedDeliveryService
    {
        Task<DeliveryApiResponse<IContent>> FetchTyped(string apiKey, Action<DeliveryQueryBuilder> builder = null);
        Task<DeliveryApiResponse<IContent>> FetchTyped(string apiKey, CancellationToken cancellationToken, Action<DeliveryQueryBuilder> builder = null);
        Task<DeliveryApiResponse> Fetch(string apiKey, Action<DeliveryQueryBuilder> builder = null);
        Task<DeliveryApiResponse> Fetch(string apiKey, CancellationToken cancellationToken, Action<DeliveryQueryBuilder> builder = null);

        /// <summary>
        /// The FetchMany is used to fetch many views by IDs or handles.
        /// Note: The maximum handles and ids combined in one request is limited to 1000. <br/>
        /// <a href="https://docs.enterspeed.com/api#tag/Delivery/operation/getContentPost">Read more </a>
        /// </summary>
        /// <param name="apiKey">Api key to validate your environment. Example: environment-1637c4d0-e878-4738-b866-152106a4f88c</param>
        /// <param name="getByIdsOrHandle">Will be turned in to the request body when posted</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DeliveryApiResponse<IContent>> FetchManyTyped(string apiKey, GetByIdsOrHandle getByIdsOrHandle, CancellationToken cancellationToken);

        /// <summary>
        /// The FetchMany is used to fetch many views by IDs or handles.
        /// Note: The maximum handles and ids combined in one request is limited to 1000. <br/>
        /// <a href="https://docs.enterspeed.com/api#tag/Delivery/operation/getContentPost">Read more </a>
        /// </summary>
        /// <param name="apiKey">Api key to validate your environment. Example: environment-1637c4d0-e878-4738-b866-152106a4f88c</param>
        /// <param name="getByIdsOrHandle">Will be turned in to the request body when posted</param>
        /// <returns></returns>
        Task<DeliveryApiResponse<IContent>> FetchManyTyped(string apiKey, GetByIdsOrHandle getByIdsOrHandle);

        /// <summary>
        /// The FetchMany is used to fetch many views by IDs or handles.
        /// Note: The maximum handles and ids combined in one request is limited to 1000. <br/>
        /// <a href="https://docs.enterspeed.com/api#tag/Delivery/operation/getContentPost">Read more </a>
        /// </summary>
        /// <param name="apiKey">Api key to validate your environment. Example: environment-1637c4d0-e878-4738-b866-152106a4f88c</param>
        /// <param name="getByIdsOrHandle">Will be turned in to the request body when posted</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DeliveryApiResponse> FetchMany(string apiKey, GetByIdsOrHandle getByIdsOrHandle, CancellationToken cancellationToken);

        /// <summary>
        /// The FetchMany is used to fetch many views by IDs or handles.
        /// Note: The maximum handles and ids combined in one request is limited to 1000. <br/>
        /// <a href="https://docs.enterspeed.com/api#tag/Delivery/operation/getContentPost">Read more </a>
        /// </summary>
        /// <param name="apiKey">Api key to validate your environment. Example: environment-1637c4d0-e878-4738-b866-152106a4f88c</param>
        /// <param name="getByIdsOrHandle">Will be turned in to the request body when posted</param>
        /// <returns></returns>
        Task<DeliveryApiResponse> FetchMany(string apiKey, GetByIdsOrHandle getByIdsOrHandle);
    }
}