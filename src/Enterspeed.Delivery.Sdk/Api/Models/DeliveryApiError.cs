using System.Net;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public class DeliveryApiError
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}