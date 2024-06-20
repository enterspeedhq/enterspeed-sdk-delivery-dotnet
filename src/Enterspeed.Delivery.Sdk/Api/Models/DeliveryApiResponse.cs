using System.Net;
using System.Net.Http.Headers;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public class DeliveryApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpResponseHeaders Headers { get; set; }
        public string Message { get; set; }
        public DeliveryResponse Response { get; set; }
    }
}