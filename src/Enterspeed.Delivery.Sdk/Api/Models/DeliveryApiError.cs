using System.Net;
using System.Net.Http.Headers;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public class DeliveryApiError
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpResponseHeaders Headers { get; set; }
        public string Message { get; set; }
    }
}