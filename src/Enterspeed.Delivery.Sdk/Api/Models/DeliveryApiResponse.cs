﻿using System.Net;

namespace Enterspeed.Delivery.Sdk.Api.Models
{
    public class DeliveryApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public DeliveryResponse Response { get; set; }
    }

    public class DeliveryApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public DeliveryResponse<T> Response { get; set; }
    }
}